using UnityEngine;
using actions;
using System;
using System.Runtime.CompilerServices;

public class ReactionNode_QuickThrow : ActionNodeBase
{
    public void SetQuickThrowPrefabPath(String PrefabPath)
    {
        quickThrowPrefabPath = PrefabPath;

        effect.SetAssetPath(quickThrowPrefabPath);
    }

    private string quickThrowPrefabPath;

    public override ActionType GetActionType => actionType;

    public override ActionType[] GetReactionType => reactions;

    public override string NameKey => nameKey;

    public override string[] PossibleParentNameKeys => possibleParentNameKeys;

    public override float MinimumInputActivationScore => minimumInputActivationScore;

    public override bool CanBeRoot => root;

    public override CombatListener Input => CombatListener.instance;

    public override ActionEffectBase Output => effect;

    private bool hasTriggered = false;

    private DroneUnitBody target;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override ActionEffectBase GetADSOutput()
    {
        if (hasTriggered == false && isTeamworkAction == false)
        {
            return Output;
        }
        else if (isTeamworkAction == true) return Output;

        return null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override float GetInputScore(CombatListener input)
    {

        float rangedBasedOnScore = caster.MyHP <= 0 ? 0f : minimumInputActivationScore + UnityEngine.Random.Range(minimumInputActivationScore/2, minimumInputActivationScore + minimumInputActivationScore/2);

        bool targetBool = targetAlly == false ? caster.IsEnemy : (caster.IsEnemy == false ? true : false);

        target = CombatListener.GetClosesTarget(targetBool, caster.transform.position);

        if (Vector3.Distance(target.transform.position, caster.transform.position) > rangedBasedOnScore) rangedBasedOnScore = -1;

        return rangedBasedOnScore;
        
    }

    public override void HandleADSOutputChain(Func<ActionEffectBase> ancestralOutputChain, int maxChainIndex, int chainIndex)
    {
        foreach (Func<ActionEffectBase> func in ancestralOutputChain.GetInvocationList())
        {
            ActionEffectBase holder = func();

            if (holder == effect)
            {
                if (target == null) continue;

                caster.ManaSpent(manaCost);

                if (caster.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Ran out of mana!");
                    continue;
                }

                holder.TriggerActionEffect(manaCost, caster, target.transform.position);

                hasTriggered = true;

                continue;
            }

            if (holder != null)
            {
                if (holder.TeamWorkTarget == null) continue;

                if (holder.TeamWorkTarget.IsEnemy == caster.IsEnemy) continue;

                caster.ManaSpent(manaCost);

                if (caster.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Ran out of mana!");
                    continue;
                }

                holder.TriggerActionEffect(manaCost, caster, holder.TeamWorkTarget.transform.position);
            }
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void WhenPutOnADSStack(CombatListener input, ActionEffectBase output)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} will react with {nameKey}!");

        hasTriggered = false;

        if (isTeamworkAction == true)
        {
            output.TeamWorkTarget = target;
        }
        else
        {
            output.TeamWorkTarget = null;
        }
    }
}
