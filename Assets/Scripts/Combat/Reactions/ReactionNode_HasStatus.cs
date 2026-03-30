using UnityEngine;
using actions;
using System;
public class ReactionNode_HasStatus : ActionNodeBase
{
    public void SetHasStatusPrefabPath(String PrefabPath)
    {
        HasStatusPrefabPath = PrefabPath;

        effect.SetAssetPath(HasStatusPrefabPath);
    }

    public void SetTargetStatus(StatusEnum target)
    {
        targetStatus = target;
    }

    private string HasStatusPrefabPath;

    public override ActionType GetActionType => actionType;

    public override ActionType[] GetReactionType => reactions;

    public override string NameKey => nameKey;

    public override string[] PossibleParentNameKeys => possibleParentNameKeys;

    public override float MinimumInputActivationScore => minimumInputActivationScore;

    public override bool CanBeRoot => root;

    public override CombatListener Input => CombatListener.instance;

    public override ActionEffectBase Output => effect;

    private StatusEnum targetStatus;

    private bool hasTriggered = false;

    private DroneUnitBody target;
    public override ActionEffectBase GetADSOutput()
    {
        if (hasTriggered == false && isTeamworkAction == false)
        {
            return Output;
        }
        else if (isTeamworkAction == true) return Output;

        return null;
    }

    public override float GetInputScore(CombatListener input)
    {
        bool targetBool = targetAlly == false ? caster.IsEnemy : (caster.IsEnemy == false ? true : false);

        target = CombatListener.GetClosesTarget(targetBool, caster.transform.position);

        if (target.GetTargetStatus(targetStatus) == true) return caster.MyHP <= 0 ? 0f : minimumInputActivationScore + 1;

        return 0;
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
