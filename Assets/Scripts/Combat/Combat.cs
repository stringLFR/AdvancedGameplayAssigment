using UnityEngine;
using ActionFlowStack;
using System.Collections.Generic;
using TMPro;


public interface ICombatObject
{
    public void CombatUpdate();
    public void OnSpawn(DroneUnitBody caster, ActionEffectBase origin);
    public void Reactivate(float mana);
    public void Reactivate(float mana, Vector3 targetPos);
    public void Reactivate(float mana, DroneUnitBody otherCaster);
    public void Reactivate(float mana, GameObject targetObj);
    public bool IsActive { get; }
    public DroneUnitBody Caster { get; }
    public ActionEffectBase Origin { get; }
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

    public static Combat instance { get; private set; }


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

        instance = null;

        instance = this;

        playerTeam = new List<DroneUnitBody>();
        enemyTeam = new List<DroneUnitBody>();
        turnOrder = new Queue<DroneUnitBody>();
        playerController = new PlayerController(this);
        aiController = new AIController(this);

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
