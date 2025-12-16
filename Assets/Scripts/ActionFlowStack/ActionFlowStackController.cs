using UnityEngine;
using ActionFlowStack;
using UnityEngine.SceneManagement;
using ADSNameSpace;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AI;
using System.Collections.Generic;
using UniGameMaths;
using static EnemySquadDataBaseSO;
using actions;
using System.Linq;


public sealed class ActionFlowStackController : MonoBehaviour
{
    [SerializeField] private PlayerTeam team;

    private string callerNameKey = "ActionFlowStackController";

    private static ActionFlowStackController instance = null;

    public static ActionFlowStackController Instance {  get { return instance; } }
    public PlayerTeam Team { get { return team; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            ActionFlowStackHandler.Callers.Add(callerNameKey);
        }

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActionFlowStackHandler.PushActionToMainStack(new FlowAction_MainMenu { });
    }

    // Update is called once per frame
    void Update()
    {
        if (instance == null || instance != this) return;

        ActionFlowStackHandler.CallUpdateMainActionFlowStack(ref callerNameKey);
    }
}

public sealed class FlowAction_MainMenu : IflowAction
{
    private AsyncOperationHandle<SceneInstance> menuScene;

    public bool IsDone()
    {
        return false;
    }

    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            //Load info before switching scenes!
            menuScene = Addressables.LoadSceneAsync("Assets/Scenes/MainMenu.unity", LoadSceneMode.Additive);
        }

        SceneRoot.SetRoot(0);
    }

    public void OnEnd()
    {
        CleanUp();
    }

    public void OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    void CleanUp()
    {
        Addressables.UnloadSceneAsync(menuScene);
        menuScene.Release();
    }
}

public sealed class FlowAction_Exploration : IflowAction
{
    private AsyncOperationHandle<SceneInstance> explorationScene;

    public bool IsDone()
    {
        throw new System.NotImplementedException();
    }

    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            //Load info before switching scenes!
            explorationScene = Addressables.LoadSceneAsync("Assets/Scenes/Exploration.unity", LoadSceneMode.Additive);
        }

        SceneRoot.SetRoot(1);
    }

    public void OnEnd()
    {
        CleanUp();
    }

    public void OnUpdate()
    {
        ActionFlowStackHandler.PushActionToMainStack(new FlowAction_Combat { });

        //throw new System.NotImplementedException();
    }

    void CleanUp()
    {
        Addressables.UnloadSceneAsync(explorationScene);
        explorationScene.Release();
    }
}

public sealed class FlowAction_Combat : IflowAction, IADSCreator<CombatListener,ActionEffectBase>
{
    private string[] combatMaps = 
    {
        "Assets/Prefabs/CombatLevels/Combat_Bridge.prefab"
    };

    public struct ADSSetupStats
    {
        public DroneUnitBody caster;
        public ActionNodeBase node;
    }

    private Combat monoCOmbat = null;
    private AsyncOperationHandle<GameObject> mapPrefab;
    private AsyncOperationHandle<GameObject> baseCharacterPrefab;
    private AsyncOperationHandle<SceneInstance> combatScene;
    private AsyncOperationHandle<EnemySquadDataBaseSO> enemyDatabase;

    public static ADS<CombatListener, ActionEffectBase> adsInstance;

    public ADS<CombatListener, ActionEffectBase> MyADS => adsInstance;

    public ADS<CombatListener, ActionEffectBase> CreateADS(int maxStackCount, int maxADSNodeCappacity) => new ADS<CombatListener, ActionEffectBase>(maxStackCount, maxADSNodeCappacity);

    public void Init(Combat c)
    {
        SceneRoot.SetRoot(2);
        monoCOmbat = c;
        Object.Instantiate(mapPrefab.Result, monoCOmbat.transform);

        List<ADSSetupStats> tempList = new List<ADSSetupStats>();

        foreach(DroneUnit unit in ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits)
        {
            Vector3 randomPoint = Vector3.zero + Random.insideUnitSphere * 50;
            NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
            GameObject obj = Object.Instantiate(baseCharacterPrefab.Result, hit.position, Quaternion.identity);
            DroneUnitBody droneUnitBody = obj.GetComponent<DroneUnitBody>();
            droneUnitBody.Init(unit, monoCOmbat.PlayerController);
            monoCOmbat.Playerteam.Add(droneUnitBody);

            if (unit.MyMainActions != null)
            {
                foreach(MainActionStats m in unit.MyMainActions)
                {
                    droneUnitBody.MainActions.Add(ActionCreator.CreateMainAction(m));
                }
            }

            if (unit.MyReactionNodes != null)
            {
                foreach (ActionNodeStats n in unit.MyReactionNodes)
                {
                    droneUnitBody.Reactions.Add(ActionCreator.CreateReactionAction(n));
                }

                foreach (ActionNodeBase node in droneUnitBody.Reactions)
                {
                    adsInstance.AddADSNode(node);
                    tempList.Add(new ADSSetupStats() {caster = droneUnitBody, node = node});
                }
            }
        }

        List<EnemySquadDatabase> enemyTeams = new List<EnemySquadDatabase>();

        foreach(EnemySquadDatabase etso in enemyDatabase.Result.Databases) if (etso.MinimumLevelRequirement <= ActionFlowStackController.Instance.Team.PlayerTeamAverageLevel) enemyTeams.Add(etso);

        EnemySquadDatabase enemyTeam = enemyTeams[Random.Range(0, enemyTeams.Count - 1)];

        EnemySquadSO enemySquadSO = enemyTeam.EnemySquads[Random.Range(0, enemyTeam.EnemySquads.Length - 1)];

        foreach (DroneUnit unit in enemySquadSO.Squad.droneUnits)
        {
            Vector3 randomPointEnemy = Vector3.zero + Random.insideUnitSphere * 50;
            NavMesh.SamplePosition(randomPointEnemy, out NavMeshHit hitEnemy, Mathf.Infinity, NavMesh.AllAreas);
            GameObject objEnemy = Object.Instantiate(baseCharacterPrefab.Result, hitEnemy.position, Quaternion.identity);
            DroneUnitBody droneUnitBodyEnemy = objEnemy.GetComponent<DroneUnitBody>();
            droneUnitBodyEnemy.Init(unit, monoCOmbat.AIController);
            monoCOmbat.EnemyTeam.Add(droneUnitBodyEnemy);

            if (unit.MyMainActions != null)
            {
                foreach (MainActionStats m in unit.MyMainActions)
                {
                    droneUnitBodyEnemy.MainActions.Add(ActionCreator.CreateMainAction(m));
                }
            }

            if (unit.MyReactionNodes != null)
            {
                foreach (ActionNodeStats n in unit.MyReactionNodes)
                {
                    droneUnitBodyEnemy.Reactions.Add(ActionCreator.CreateReactionAction(n));
                }

                foreach (ActionNodeBase node in droneUnitBodyEnemy.Reactions)
                {
                    adsInstance.AddADSNode(node);
                    tempList.Add(new ADSSetupStats() { caster = droneUnitBodyEnemy, node = node });
                }
            }
        }

        foreach(ADSSetupStats nodeStats in tempList) //TODO TRY TO FIND BETTER WAY TO DO THIS!
        {
            ADSSetupStats[] nodes = tempList.FindAll(c => c.node.GetReactionType.Contains(nodeStats.node.GetActionType) == true && c.node.NameKey != nodeStats.node.NameKey).ToArray();

            string[] names = new string[nodes.Length];

            for (int i = 0; i < nodes.Length; i++) names[i] = nodes[i].node.NameKey;

            nodeStats.node.SetupNode(nodeStats.caster, names);
        }
    }

    public bool IsDone()
    {
        return false;
    }

    public void OnBegin(bool bFirstTime) 
    {
        
        if (bFirstTime)
        {
            
            int randomIndex = Random.Range(0, combatMaps.Length - 1);
            string index = combatMaps[randomIndex];
            //Load info before switching scenes!
            CombatListener.Init();
            adsInstance = CreateADS(10, 100);
            mapPrefab = Addressables.LoadAssetAsync<GameObject>(index);
            baseCharacterPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Characters/ProcedualBody.prefab");
            enemyDatabase = Addressables.LoadAssetAsync<EnemySquadDataBaseSO>("Assets/SOs/EnemySquadDatabase.asset");
            mapPrefab.WaitForCompletion();
            baseCharacterPrefab.WaitForCompletion();
            enemyDatabase.WaitForCompletion();
            combatScene = Addressables.LoadSceneAsync("Assets/Scenes/Combat.unity", LoadSceneMode.Additive);
            
        }
    }

    public void OnEnd()
    { 
        cleanUp();
    }

    public void OnUpdate()
    {
        if (monoCOmbat == null) return;

        monoCOmbat.Tick();
        CombatListener.Tick(monoCOmbat);
    }

    void cleanUp()
    {
        CombatListener.CleanUp();
        mapPrefab.Release();
        Addressables.UnloadSceneAsync(combatScene);
        combatScene.Release();
        baseCharacterPrefab.Release();
        enemyDatabase.Release();
        Combat.actionEffectObjects.Clear();
        adsInstance = null;
    }
}

public sealed class FlowAction_PauseMenu : IflowAction
{
    public bool IsDone()
    {
        throw new System.NotImplementedException();
    }

    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            //Load info before switching scenes!
        }
    }

    public void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void GoToMainMenu()
    {
        FlowAction_MainMenu menu = new FlowAction_MainMenu();
        IflowAction[] arr = { menu };
        ActionFlowStackHandler.ReplaceMainStack(arr);
    }
}

public sealed class FlowAction_OptionsMenu : IflowAction
{
    public bool IsDone()
    {
        throw new System.NotImplementedException();
    }

    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            //Load info before switching scenes!
        }
    }

    public void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}

public sealed class FlowAction_Turn : IflowAction
{
    private Combat monoCombat;
    private DroneUnitBody turnHolder;
    private bool isDone = false;
    private Vector3 userPos;

    public void Init(Combat c, DroneUnitBody d)
    {
        monoCombat = c;
        turnHolder = d;
    }

    public bool IsDone()
    {
        return isDone;
    }

    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            userPos = turnHolder.transform.position;
            userPos.y = monoCombat.BattleCamera.transform.position.y;
            turnHolder.Controller.ControllerEnable(turnHolder);
        }
        
    }

    public void OnEnd()
    {
        turnHolder.Controller.ControllerDisable(turnHolder);
    }

    private float time = 0;

    public void OnUpdate()
    {
        if (monoCombat == null) return;

        CombatListener.Tick(monoCombat);

        if (turnHolder.Controller.isDone == true)
        {
            isDone = true;
            return;
        }

        if (time < 1)
        {
            userPos.y = monoCombat.BattleCamera.transform.position.y;
            time += Time.deltaTime;
            monoCombat.BattleCamera.transform.position =
                Vector3.LerpUnclamped(
                    monoCombat.BattleCamera.transform.position, userPos,
                    EasingFunctionMaths.EaseInSine(time)
                    );
        }
    }
}
