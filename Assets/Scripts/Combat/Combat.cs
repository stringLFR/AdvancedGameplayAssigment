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
    private TextMeshProUGUI testTurnText;

    private Queue<DroneUnitBody> turnOrder;

    private PlayerController playerController;
    private AIController aiController;

    public PlayerController PlayerController => playerController;
    public AIController AIController => aiController;   


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

        playerTeam = new List<DroneUnitBody>();
        enemyTeam = new List<DroneUnitBody>();
        turnOrder = new Queue<DroneUnitBody>();
        playerController = new PlayerController(this);
        aiController = new AIController(this);

        PlayerHud.SetActive(false);

        print("Combat Found!");
        combatFlowAction.Init(this);

        int biggestTeamSize = playerTeam.Count >= enemyTeam.Count ? playerTeam.Count : enemyTeam.Count;

        for (int i = 0; i < biggestTeamSize; i++)
        {
            if (playerTeam[i] == null && enemyTeam[i] == null) break;
            else if (playerTeam[i] == null && enemyTeam[i] != null) turnOrder.Enqueue(enemyTeam[i]);
            else if (playerTeam[i] != null && enemyTeam[i] == null) turnOrder.Enqueue(playerTeam[i]);
            else if (playerTeam[i].MySpeed >= enemyTeam[i].MySpeed)
            {
                turnOrder.Enqueue(playerTeam[i]);
                turnOrder.Enqueue(enemyTeam[i]);
            }
            else if (playerTeam[i].MySpeed < enemyTeam[i].MySpeed)
            {
                turnOrder.Enqueue(enemyTeam[i]);
                turnOrder.Enqueue(playerTeam[i]);
            }
        }

        //Stack array! No HEap alloc!
        //Span<int> arr = stackalloc int[4];
    }

    public void GetNextTurnHolder()
    {
        DroneUnitBody body = turnOrder.Dequeue();
        turnOrder.Enqueue(body);
        testTurnText.text = body.DroneUnit.DroneName;
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

        for(int i = 0; i < actionEffectObjects.Count; i++)
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
