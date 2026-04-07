using ActionFlowStack;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public enum ICombatDelegateTriggers
{
    NONE, ON_REACTIVATE, ON_FINISHED, ON_DRONEHIT,
}

public interface ICombatObject
{
    public event Action<ICombatObject> MyActionDelegate;

    public void TriggerDelegate();
    public float myDamageType { get; set; }
    public float RespondActionMana { get; set; }
    public DroneUnitBody RespondActionTarget { get; set; }
    public void MyRespondAction(ICombatObject obj);
    public void CombatUpdate();
    public void OnSpawn(DroneUnitBody caster, ActionEffectBase origin, ICombatDelegateTriggers delegateTrigger);
    public void Reactivate(float mana);
    public void Reactivate(float mana, Vector3 targetPos);
    public void Reactivate(float mana, DroneUnitBody otherCaster);
    public void Reactivate(float mana, GameObject targetObj);
    public bool FinalEffectReturnValue();
    public bool FinalEffectReturnValue(Vector3 triggerPos);
    public bool FinalEffectReturnValue(DroneUnitBody triggeredDrone);
    public bool FinalEffectReturnValue(GameObject triggeredObject);
    public bool IsActive { get; }
    public DroneUnitBody Caster { get; }
    public ActionEffectBase Origin { get; }
    public ICombatDelegateTriggers MyDelegateTriggerType { get; }
}

public sealed class Combat : MonoBehaviour
{
    [SerializeField]
    private GameObject CRPGCamera;
    [SerializeField]
    private Canvas battleCanvas;

    [SerializeField]
    private GameObject PlayerHud;

    [SerializeField]
    private TextMeshProUGUI combatText;

    [SerializeField]
    private int combatTextLineCountMax = 5;

    public int CombatTextLineCountMax => combatTextLineCountMax;

    public TextMeshProUGUI CombatText => combatText;

    private Queue<DroneUnitBody> turnOrder;

    private PlayerController playerController;
    private AIController aiController;

    public PlayerController PlayerController => playerController;
    public AIController AIController => aiController;   

    public static Transform instanceTransfrom { get; private set; }


    private List<DroneUnitBody> playerTeam;
    private List<DroneUnitBody> enemyTeam;
    public GameObject PlayerHUD => PlayerHud;
    public List<DroneUnitBody> Playerteam { get { return playerTeam; } }
    public List<DroneUnitBody> EnemyTeam { get { return enemyTeam; } }

    public static List<ICombatObject> actionEffectObjects = new List<ICombatObject>();

    public GameObject BattleCamera => CRPGCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        FlowAction_Combat combatFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Combat;

        if (combatFlowAction == null) return;

        instanceTransfrom = null;

        instanceTransfrom = this.transform;

        playerTeam = new List<DroneUnitBody>();
        enemyTeam = new List<DroneUnitBody>();
        turnOrder = new Queue<DroneUnitBody>();
        playerController = new PlayerController(this);
        aiController = new AIController(this);
        aiController.InitAI();

        PlayerHud.SetActive(false);

        print("Combat Found!");
        combatFlowAction.Init(this);

        int combatants = playerTeam.Count + enemyTeam.Count;

        DroneUnitBody[] arr = new DroneUnitBody[combatants];

        playerTeam.CopyTo(arr, 0);
        enemyTeam.CopyTo(arr, playerTeam.Count);

        for (int i = 0; i < combatants; i++)
        {
            float best = 0;
            int index = i; 
            for(int j = 0; j < arr.Length; j++)
            {
                if (arr[j] == null) continue;

                if (arr[j].MySpeed >= best)
                {
                    best = arr[j].MySpeed;
                    index = j;
                }
            }

            turnOrder.Enqueue(arr[index]);
            arr[index] = null;
        }

        //Stack array! No HEap alloc!
        //Span<int> arr = stackalloc int[4];
        SetupAIDecisions();
    }

    private void SetupAIDecisions()
    {
        foreach(DroneUnitBody enemy in enemyTeam)
        {
            foreach(MainActionBase action in enemy.MainActions)
            {
                MainActionBase[] nodes = enemy.MainActions.FindAll(c => c.ReactionTypes.Contains(action.ActionType) == true && c.MainActionName != action.MainActionName).ToArray();

                string[] names = new string[nodes.Length];

                for (int i = 0; i < nodes.Length; i++) names[i] = nodes[i].MainActionName;

                AI_DecisionBase aiBase = null;

                switch (action.AIDecisionType)
                {
                    case AIDecisionType.NONE:

                        return; 
                }

                aiBase.InitAIDecision(action, enemy, aiController);
                aiBase.AddParents(names);
                aiController.MyADS.AddADSNode(aiBase);
            }
        }
    }

    public void GetNextTurnHolder()
    {
        DroneUnitBody body = turnOrder.Dequeue();

        if (body.MyHP <= 0)
        {
            GetNextTurnHolder();
            return;
        }

        turnOrder.Enqueue(body);
        CombatListener.AddLineToCombatText(body.DroneUnit.DroneName + " Takes their turn!");
        CombatListener.AddLineToCombatText($"With {turnOrder.Peek().DroneUnit.DroneName} going after them!");
        FlowAction_Turn t = new FlowAction_Turn();
        t.Init(this, body);
        ActionFlowStackHandler.PushActionToStack(t);
    }

    float time = 0;

    public void Tick() //Update called from actionFlowStack!
    {
        print("COMBAT");
        time += Time.deltaTime;

        if (time > 2)
        {
            time = 0;
            GetNextTurnHolder();
        }


        //TODO: CHANGE TO NativeArray and use multithreading (Job system?)!
        //https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Unity.Collections.NativeArray_1.html
        for (int i = 0; i < actionEffectObjects.Count; i++)
        {
            if (actionEffectObjects[i].IsActive == false)
            {
                actionEffectObjects.Remove(actionEffectObjects[i]);
                continue;
            }

            actionEffectObjects[i].CombatUpdate();
        }
    }
}
