using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using ADSNameSpace;
using System;

public enum AIDecisionType //Only works with AIController!
{
    NONE, Find_MostWounded, Find_MostHealthy, Find_MostBuffed, Find_MostDebuffed, Find_StatusInflicted,
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

    public AI_DecisionBase lastDecisionOnStack;

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
        lastDecisionOnStack = null;
        turnHolder = user;
        Debug.Log("corutine Start");
        yield return new WaitForSeconds(0.5f);

        AIADS.CreateAncestryStack();

        AIADS.ActivateAncestryChain();

        yield return new WaitForSeconds(0.5f);

        isdone = true;
        turnHolder = null;
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
    protected Vector3 myTargetPos; 

    public string NameKey => nameKey;

    public string[] PossibleParentNameKeys => parents;

    public float MinimumInputActivationScore => minScore;

    public bool CanBeRoot => user == controller.TurnHolder;

    public CombatListener Input => CombatListener.instance;

    public MainActionBase Output => myAction;

    public Vector3 MyTargetPos => myTargetPos;


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
        float priorityInput = 0f;
        float baseInput = GetInputValue(input);
        float manaInput = Mathf.Clamp((user.MyMana - myAction.ManaCost) / user.MyMaxMana,0, user.MyMaxMana);
        float randomInput = UnityEngine.Random.Range(-0.10f, 0.10f);

        if (controller.lastDecisionOnStack != null && controller.lastDecisionOnStack.MyTargetPos == myTargetPos) priorityInput = UnityEngine.Random.Range(0.10f, 0.20f);

        return ((baseInput * 100) / Vector3.Distance(user.transform.position, myTargetPos)) + randomInput + priorityInput + manaInput / 2;
    }

    public virtual void HandleADSOutputChain(Func<MainActionBase> ancestralOutputChain, int maxChainIndex, int chainIndex)
    {
        if (user.Overdrive < chainIndex) return;

        controller.AITargetPos = myTargetPos;

        myAction.Activate(controller, user);

        controller.HasOverdrive(user);
    }

    public virtual void WhenPutOnADSStack(CombatListener input, MainActionBase output)
    {
        controller.lastDecisionOnStack = this;
    }
}

public class AI_Decision_Find_MostWounded : AI_DecisionBase
{
    protected override float GetInputValue(CombatListener input)
    {
        DroneUnitBody[] targetTeam = user.IsEnemy == false ? CombatListener.Combat.EnemyTeam.ToArray() : CombatListener.Combat.Playerteam.ToArray();

        int mostWoundedIndex = -1;
        int currentHP = int.MaxValue;
        int index = -1;

        foreach (var hp in targetTeam)
        {
            index++;
            if (hp.MyHP >= currentHP)
            {
                continue;
            }
            currentHP = hp.MyHP;
            mostWoundedIndex = index;
        }

        if (mostWoundedIndex < 0) return -1;

        myTargetPos = targetTeam[mostWoundedIndex].transform.position;

        return targetTeam[mostWoundedIndex].MyMaxHP - (targetTeam[mostWoundedIndex].MyHP / targetTeam[mostWoundedIndex].MyMaxHP);
    }

    protected override float SetMinScore()
    {
        return 0.25f;
    }
}

public class AI_Decision_Find_MostHealthy : AI_DecisionBase
{
    protected override float GetInputValue(CombatListener input)
    {
        DroneUnitBody[] targetTeam = user.IsEnemy == false ? CombatListener.Combat.EnemyTeam.ToArray() : CombatListener.Combat.Playerteam.ToArray();

        int mostHealthyIndex = -1;
        int currentHP = int.MinValue;
        int index = -1;

        foreach (var hp in targetTeam)
        {
            index++;
            if (hp.MyHP <= currentHP)
            {
                continue;
            }
            currentHP = hp.MyHP;
            mostHealthyIndex = index;
        }

        if (mostHealthyIndex < 0) return -1;

        myTargetPos = targetTeam[mostHealthyIndex].transform.position;

        return (targetTeam[mostHealthyIndex].MyHP / targetTeam[mostHealthyIndex].MyMaxHP);
    }

    protected override float SetMinScore()
    {
        return 0.25f;
    }
}

public class AI_Decision_Find_MostBuffed : AI_DecisionBase
{
    protected override float GetInputValue(CombatListener input)
    {
        DroneUnitBody[] targetTeam = user.IsEnemy == false ? CombatListener.Combat.EnemyTeam.ToArray() : CombatListener.Combat.Playerteam.ToArray();

        int mostBuffedIndex = -1;
        int currentBuffValue = int.MinValue;
        int index = -1;

        foreach (var hp in targetTeam)
        {
            index++;
            if (hp.GetTargetBuffValue((BuffsEnum)myAction.EnumTarget) <= currentBuffValue)
            {
                continue;
            }
            currentBuffValue = hp.GetTargetBuffValue((BuffsEnum)myAction.EnumTarget);
            mostBuffedIndex = index;
        }

        if (mostBuffedIndex < 0) return -1;

        myTargetPos = targetTeam[mostBuffedIndex].transform.position;

        return currentBuffValue / currentBuffValue + 1;
    }

    protected override float SetMinScore()
    {
        return 0.25f;
    }
}

public class AI_Decision_Find_MostDebuffed : AI_DecisionBase
{
    protected override float GetInputValue(CombatListener input)
    {
        DroneUnitBody[] targetTeam = user.IsEnemy == false ? CombatListener.Combat.EnemyTeam.ToArray() : CombatListener.Combat.Playerteam.ToArray();

        int mostDebuffedIndex = -1;
        int currentDebuffValue = int.MinValue;
        int index = -1;

        foreach (var hp in targetTeam)
        {
            index++;
            if (hp.GetTargetDebuffValue((DebuffsEnum)myAction.EnumTarget) <= currentDebuffValue)
            {
                continue;
            }
            currentDebuffValue = hp.GetTargetDebuffValue((DebuffsEnum)myAction.EnumTarget);
            mostDebuffedIndex = index;
        }

        if (mostDebuffedIndex < 0) return -1;

        myTargetPos = targetTeam[mostDebuffedIndex].transform.position;

        return currentDebuffValue / currentDebuffValue + 1;
    }

    protected override float SetMinScore()
    {
        return 0.25f;
    }
}

public class AI_Decision_Find_StatusInflicted : AI_DecisionBase
{
    protected override float GetInputValue(CombatListener input)
    {
        DroneUnitBody[] targetTeam = user.IsEnemy == false ? CombatListener.Combat.EnemyTeam.ToArray() : CombatListener.Combat.Playerteam.ToArray();

        int mostStatusIndex = -1;
        int index = -1;

        foreach (var hp in targetTeam)
        {
            index++;
            if (!hp.GetTargetStatus((StatusEnum)myAction.EnumTarget))
            {
                continue;
            }
            mostStatusIndex = index;
        }

        if (mostStatusIndex < 0) return -1;

        myTargetPos = targetTeam[mostStatusIndex].transform.position;

        float returnValue = 0f;

        switch ((StatusEnum)myAction.EnumTarget)
        {
            case StatusEnum.None:
                break;
            case StatusEnum.Stunned:

                returnValue = 0.7f;

                break;
            case StatusEnum.Kncokback:

                returnValue = 0.8f;

                break;
            case StatusEnum.Leaking:

                returnValue = 0.99f;

                break;
            case StatusEnum.Negation:

                returnValue = 0.51f;

                break;
            case StatusEnum.Hacked:

                returnValue = 0.9f;

                break;
            case StatusEnum.Manaburn:

                returnValue = 0.6f;

                break;
        }

        return returnValue;
    }

    protected override float SetMinScore()
    {
        return 0.25f;
    }
}
