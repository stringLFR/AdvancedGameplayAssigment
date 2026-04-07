using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using ADSNameSpace;
using System;

public enum AIDecisionType //Only works with AIController!
{
    NONE,
}

public class AIController : ControllerBase, IADSCreator<CombatListener,MainActionBase>
{
    private Combat monoCombat;
    private bool isdone = false;

    private int overdrives = 0;
    private ADS<CombatListener, MainActionBase> AIADS;

    public Vector3 AITargetPos;

    public override bool isDone => isdone;

    public ADS<CombatListener, MainActionBase> MyADS => AIADS;

    public int Overdrives => overdrives;

    private DroneUnitBody turnHolder = null;

    public DroneUnitBody TurnHolder => turnHolder;

    public void InitAI()
    {
        AIADS = CreateADS(5, 50);
    }

    public override void ControllerDisable(DroneUnitBody user)
    {
        Debug.Log("AI TURN END");
        isdone = false;
    }

    public override void ControllerEnable(DroneUnitBody user)
    {
        Debug.Log("AI TURN START");
        monoCombat.StartCoroutine(CalculateAction(user));
    }

    private IEnumerator CalculateAction(DroneUnitBody user)
    {
        overdrives = 0;
        Debug.Log("corutine Start");
        yield return new WaitForSeconds(1);

        //AIADS.CreateAncestryStack();

        //AIADS.ActivateAncestryChain();

        isdone = true;
        Debug.Log("corutine end");
    }

    public AIController(Combat c)
    {
        monoCombat = c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public bool HasOverdrive(DroneUnitBody user)
    {
        if (user.Overdrive > overdrives)
        {
            CombatListener.AddLineToCombatText(user.DroneUnit.DroneName + $" has overdrive {user.Overdrive}! " +
                $"They can take {user.Overdrive - overdrives} more main actions!");

            overdrives++;

            if (UnityEngine.Random.Range(0f, 1f) > 1f - overdrives / user.Overdrive) user.SanityDamage(user.Overdrive);

            return true;
        }

        return false;
    }

    public ADS<CombatListener, MainActionBase> CreateADS(int maxStackCount, int maxADSNodeCappacity)
    {
        return new ADS<CombatListener, MainActionBase>(maxStackCount, maxADSNodeCappacity);
    }
}

public abstract class AI_DecisionBase : IADSNode<CombatListener, MainActionBase>
{

    protected string nameKey;
    protected MainActionBase myAction;
    protected DroneUnitBody user;
    protected AIController controller;
    protected string[] parents = null;
    protected float minScore;

    public string NameKey => nameKey;

    public string[] PossibleParentNameKeys => parents;

    public float MinimumInputActivationScore => minScore;

    public bool CanBeRoot => user == controller.TurnHolder;

    public CombatListener Input => CombatListener.instance;

    public MainActionBase Output => myAction;


    public void InitAIDecision(MainActionBase mainAction, DroneUnitBody unit, AIController aIController)
    {
        nameKey = mainAction.MainActionName;
        myAction = mainAction;
        controller = aIController;
        user = unit;
        minScore = SetMinScore();
    }

    public void AddParents(string[] parentKeyArr)
    {
        parents = parentKeyArr;
    }

    protected abstract float SetMinScore();

    protected abstract float GetInputValue(CombatListener input);

    public virtual MainActionBase GetADSOutput()
    {
        return Output;
    }

    public virtual float GetInputScore(CombatListener input)
    {
        return GetInputValue(input);
    }

    public virtual void HandleADSOutputChain(Func<MainActionBase> ancestralOutputChain, int maxChainIndex, int chainIndex)
    {
        if (user.Overdrive < chainIndex) return;

        myAction.Activate(controller, user);

        controller.HasOverdrive(user);
    }

    public virtual void WhenPutOnADSStack(CombatListener input, MainActionBase output)
    {
        
    }
}
