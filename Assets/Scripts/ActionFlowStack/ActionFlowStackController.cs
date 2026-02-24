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
using System.Runtime.CompilerServices;

public enum CombatState
{
    NOT_IN_COMBAT, IN_COMBAT, WON, LOST, FLEED,
}

public sealed class ActionFlowStackController : MonoBehaviour
{
    [SerializeField] private PlayerTeam team;
    [SerializeField] private PauseMenu pauseMenu;

    private string callerNameKey = "ActionFlowStackController";

    private static ActionFlowStackController instance = null;

    public static ActionFlowStackController Instance {  get { return instance; } }
    public PlayerTeam Team { get { return team; } }

    public CombatState CombatState { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            ActionFlowStackHandler.Callers.Add(callerNameKey);
            pauseMenu.gameObject.SetActive(false);
        }

        
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetCombatState(CombatState newState)
    {
        CombatState = newState;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActionFlowStackHandler.PushActionToStack(new FlowAction_MainMenu { });
    }

    // Update is called once per frame
    void Update()
    {
        if (instance == null || instance != this) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.gameObject.activeInHierarchy == true)
            {

                pauseMenu.UnPause();

                pauseMenu.gameObject.SetActive(false);
            }
            else
            {
                pauseMenu.gameObject.SetActive(true);

                if (ActionFlowStackHandler.CurrentFlowAction is FlowAction_Exploration)
                {
                    FlowAction_Exploration e = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Exploration;

                    e.StopBodies();
                }

                FlowAction_PauseMenu p = new FlowAction_PauseMenu();

                ActionFlowStackHandler.PushActionToStack(p);

                pauseMenu.ActivatePause(p);
            }
        }

        ActionFlowStackHandler.CallUpdateMainActionFlowStack(ref callerNameKey);
    }
}

public sealed class FlowAction_MainMenu : IflowAction
{
    private AsyncOperationHandle<SceneInstance> menuScene;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool IsDone()
    {
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            //Load info before switching scenes!
            menuScene = Addressables.LoadSceneAsync("Assets/Scenes/MainMenu.unity", LoadSceneMode.Additive);
        }

        SceneRoot.SetRoot(0);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnEnd()
    {
        CleanUp();
    }

    public void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    void CleanUp()
    {
        Addressables.UnloadSceneAsync(menuScene);
        menuScene.Release();
    }
}

public sealed class FlowAction_Exploration : IflowAction
{
    private AsyncOperationHandle<SceneInstance> explorationScene;

    private Exploration expo = null;
    private List<Exploration_Caravan> caravans;
    private List<Exploration_Hostile> hostiles;
    private List<Exploration_Node> nodes;

    public void RemoveNode(Exploration_Node target)
    {
        nodes.Remove(target);
    }

    private int victoryPoints = 0;

    private const int MaxVictoryPoints = 3;

    public void AddVictoryPoint()
    {
        victoryPoints++;

        expo.WinScoreText.text = $"Win Score<br>{victoryPoints}/{MaxVictoryPoints}";

        if (victoryPoints == MaxVictoryPoints)
        {
            FlowAction_GameOver game = new FlowAction_GameOver();
            game.SetGameOverState(true, expo);
            ActionFlowStackHandler.PushActionToStack(game);
        }
    }

    float time = 0f;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool IsDone()
    {
        return false;
    }

    public void Init(Exploration e)
    {
        expo = e;
        expo.WinScoreText.text = $"Win Score<br>{victoryPoints}/{MaxVictoryPoints}";
        e.MapSetup(nodes);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SpawnCaravan(Exploration_Node target)
    {
        caravans.Add(new Exploration_Caravan().SpawnCaravan(expo, target));

        expo.SetImagesCaravan(caravans.Count);
    }

    public void SpawnHostiles(int amount, Vector3 pos, float radius)
    {
        for (int i = 0; i < amount; i++)
        {
            //Using amount for hp for now... May not be a good idea XD (IF many spawn at ones, the hp will be very high!)
            hostiles.Add(new Exploration_Hostile(expo.Explorer).SpawnHostile(expo, pos + Random.insideUnitSphere * radius, amount));
        }

        expo.SetImagesHostile(hostiles.Count);
    }

    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            victoryPoints = 0;
            hostiles = new List<Exploration_Hostile>();
            caravans = new List<Exploration_Caravan>();
            nodes = new List<Exploration_Node>();
            //Load info before switching scenes!
            explorationScene = Addressables.LoadSceneAsync("Assets/Scenes/Exploration.unity", LoadSceneMode.Additive);
        }
        SceneRoot.SetRoot(1);

        foreach (Exploration_Hostile h in hostiles)
        {
            if (h.body.ProcedualCore.Root.tr.gameObject.activeInHierarchy == false) h.body.ProcedualCore.Root.tr.gameObject.SetActive(true);

            h.body.ProcedualCore.Agent.isStopped = false;
        }

        foreach (Exploration_Caravan c in caravans)
        {
            if (c.body.ProcedualCore.Root.tr.gameObject.activeInHierarchy == false) c.body.ProcedualCore.Root.tr.gameObject.SetActive(true);

            c.body.ProcedualCore.Agent.isStopped = false;
        }

        if (expo == null) return;

        expo.Explorer.ProcedualCore.Agent.isStopped = false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnEnd()
    {
        CleanUp();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnUpdate()
    {
        if (expo == null) return;

        time += Time.deltaTime;

        if (time >= hostiles.Count * 2)
        {
            time = 0f;

            SpawnHostiles(UnityEngine.Random.Range(1, hostiles.Count + 1), expo.transform.position + Random.insideUnitSphere * 500f, UnityEngine.Random.Range(1f,100f));

            foreach (Exploration_Hostile h in hostiles)
            {
                h.SetHostileDestination(expo, nodes,caravans);
            }
        }

        expo.Tick(caravans, hostiles, nodes);

        //ActionFlowStackHandler.PushActionToMainStack(new FlowAction_Combat { });

        //throw new System.NotImplementedException();
    }

    public void StopBodies()
    {
        foreach (Exploration_Hostile h in hostiles)
        {
            h.body.ProcedualCore.Agent.isStopped = true;
        }

        foreach (Exploration_Caravan c in caravans)
        {
            c.body.ProcedualCore.Agent.isStopped = true;
        }
        expo.Explorer.ProcedualCore.Agent.isStopped = true;
    }

    void CleanUp()
    {
        foreach(Exploration_Node n in nodes)
        {
            n.CleanUpNode();
        }

        foreach(Exploration_Hostile h in hostiles)
        {
            Object.Destroy(h.body.gameObject);
        }

        foreach (Exploration_Caravan c in caravans)
        {
            Object.Destroy(c.body.gameObject);
        }

        Exploration_Management.Activated = false;
        expo.positions.Dispose();
        expo.rotations.Dispose();
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

    private bool combatDone = false;

    public void Init(Combat c)
    {
        SceneRoot.SetRoot(2);
        monoCOmbat = c;
        CombatListener.Init(c);
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
            droneUnitBody.SetEnemyBool(false);

            if (unit.MyMainActions != null)
            {
                foreach(MainActionStats m in unit.MyMainActions)
                {
                    droneUnitBody.MainActions.Add(ActionCreator.CreateMainAction(m, droneUnitBody.DroneUnit.DroneName));
                }
            }

            if (unit.MyReactionNodes != null)
            {
                foreach (ActionNodeStats n in unit.MyReactionNodes)
                {
                    droneUnitBody.Reactions.Add(ActionCreator.CreateReactionAction(n, droneUnitBody.DroneUnit.DroneName));
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
            droneUnitBodyEnemy.SetEnemyBool(true);

            if (unit.MyMainActions != null)
            {
                foreach (MainActionStats m in unit.MyMainActions)
                {
                    droneUnitBodyEnemy.MainActions.Add(ActionCreator.CreateMainAction(m, droneUnitBodyEnemy.DroneUnit.DroneName));
                }
            }

            if (unit.MyReactionNodes != null)
            {
                foreach (ActionNodeStats n in unit.MyReactionNodes)
                {
                    droneUnitBodyEnemy.Reactions.Add(ActionCreator.CreateReactionAction(n, droneUnitBodyEnemy.DroneUnit.DroneName));
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool IsDone()
    {
        return combatDone;
    }

    public void CombatOver(List<DroneUnitBody> winningTeam = null)
    {
        combatDone = true;

        for (int i = 0; i < winningTeam.Count; i++)
        {
            if (winningTeam[i].MyHP <= 0)
            {
                for (int j = 0; j < ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits.Length; j++)
                {
                    if (winningTeam[i].DroneUnit == ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits[j])
                    {
                        ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits[j] = null;
                        break;
                    }
                }
            }

            winningTeam[i].DroneUnit.afterCombatStats.HPdamageTakenPercentile = winningTeam[i].MyMaxHP - winningTeam[i].MyHP;
        }

        if (winningTeam != null) //Player won!
        {
            ActionFlowStackController.Instance.SetCombatState(CombatState.WON);
            return;
        }

        ActionFlowStackController.Instance.SetCombatState(CombatState.LOST);
    }

    public void OnBegin(bool bFirstTime) 
    {
        
        if (bFirstTime)
        {
            
            int randomIndex = Random.Range(0, combatMaps.Length - 1);
            string index = combatMaps[randomIndex];
            //Load info before switching scenes!
            adsInstance = CreateADS(10, 100);
            mapPrefab = Addressables.LoadAssetAsync<GameObject>(index);
            baseCharacterPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Characters/ProcedualBody.prefab");
            enemyDatabase = Addressables.LoadAssetAsync<EnemySquadDataBaseSO>("Assets/SOs/EnemySquadDatabase.asset");
            mapPrefab.WaitForCompletion();
            baseCharacterPrefab.WaitForCompletion();
            enemyDatabase.WaitForCompletion();
            combatScene = Addressables.LoadSceneAsync("Assets/Scenes/Combat.unity", LoadSceneMode.Additive);
            
        }

        ActionFlowStackController.Instance.SetCombatState(CombatState.IN_COMBAT);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnEnd()
    {
       
        cleanUp();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
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
    private bool done;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool IsDone()
    {
        return done;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            done = false;
        }
    }

    public void SetDone() => done = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnEnd()
    {
        
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnUpdate()
    {
        
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public static void GoToMainMenu()
    {
        FlowAction_MainMenu menu = new FlowAction_MainMenu();
        IflowAction[] arr = { menu };
        ActionFlowStackHandler.ReplaceStack(arr);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Init(Combat c, DroneUnitBody d)
    {
        monoCombat = c;
        turnHolder = d;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool IsDone()
    {
        return isDone;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            userPos = turnHolder.transform.position;
            userPos.y = monoCombat.BattleCamera.transform.position.y;
            turnHolder.Controller.ControllerEnable(turnHolder);
        }
        
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
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

public sealed class FlowAction_Management : IflowAction
{
    private bool isDone = false;

    private AsyncOperationHandle<GameObject> managementPrefab;
    private Exploration_Management management;
    private Exploration expo;

    public Exploration Expo => expo;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Init(Exploration_Management m, Exploration e)
    {
        management = m;
        expo = e;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void GoExploring()
    {
        isDone = true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool IsDone()
    {
        return isDone;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnBegin(bool bFirstTime)
    {
        if (bFirstTime)
        {
            managementPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Management.prefab");
            managementPrefab.WaitForCompletion();
            Object.Instantiate(managementPrefab.Result);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnEnd()
    {
        CleanUp();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnUpdate()
    {
        if (management == null) return;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    void CleanUp()
    {
        managementPrefab.Release();
        Object.Destroy(management.gameObject);
    }

    
}

public sealed class FlowAction_GameOver : IflowAction
{

    private bool wonBoolean;
    private Exploration expo = null;
    private string gameOverText;

    public void SetGameOverState(bool won, Exploration e)
    {
        wonBoolean = won;
        expo = e;

        if (wonBoolean == true)
        {
            gameOverText = "YOU WON!";
        }
        else
        {
            gameOverText = "YOU LOST!";
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
            expo.GameOverScreen.gameObject.SetActive(true);

            expo.GameOverScreenButton.onClick.AddListener(() => { FlowAction_PauseMenu.GoToMainMenu(); });

            expo.GameOverScreenText.text = gameOverText;
        }
    }

    public void OnEnd()
    {
        expo.GameOverScreenButton.onClick.RemoveAllListeners();

    }

    public void OnUpdate()
    {

    }
}


