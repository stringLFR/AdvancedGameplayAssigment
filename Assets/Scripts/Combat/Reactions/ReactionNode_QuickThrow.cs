using UnityEngine;
using actions;
using System;
using System.Runtime.CompilerServices;

public class ReactionNode_QuickThrow : ActionNodeBase
{
    public void SetQuickTrhowPrefabPath(String PrefabPath)
    {
        quickTrhowPrefabPath = PrefabPath;

        if (effect is AE_ShotProjectile)
        {
            AE_ShotProjectile s = effect as AE_ShotProjectile;
            s.SetAssetPath(quickTrhowPrefabPath);
        }

        if (effect is AE_SwingMelee)
        {
            AE_SwingMelee m = effect as AE_SwingMelee;
            m.SetAssetPath(quickTrhowPrefabPath);
        }
    }

    private string quickTrhowPrefabPath;

    public override ActionType GetActionType => actionType;

    public override ActionType[] GetReactionType => reactions;

    public override string NameKey => nameKey;

    public override string[] PossibleParentNameKeys => possibleParentNameKeys;

    public override float MinimumInputActivationScore => minimumInputActivationScore;

    public override bool CanBeRoot => root;

    public override CombatListener Input => CombatListener.instance;

    public override ActionEffectBase Output => effect;

    private bool hasTriggered = false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override ActionEffectBase GetADSOutput()
    {
        if (hasTriggered == false)
        {
            hasTriggered = true;
            return Output;
        }

        return null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override float GetInputScore(CombatListener input)
    {
        float score = caster.MyHP <= 0 ? 0f : UnityEngine.Random.Range(minimumInputActivationScore / 2, minimumInputActivationScore + minimumInputActivationScore / 2);

        if (caster.MyHP <= 0) score = -1;
        
        return score;
        
    }

    public override void HandleADSOutputChain(Func<ActionEffectBase> ancestralOutputChain, int maxChainIndex, int chainIndex)
    {
        foreach (Func<ActionEffectBase> func in ancestralOutputChain.GetInvocationList())
        {
            ActionEffectBase holder = func();

            if (holder == effect)
            {
                DroneUnitBody target = CombatListener.GetClosesTarget(caster.IsEnemy, caster.transform.position);

                if (target == null) return;

                caster.ManaSpent(manaCost);

                if (caster.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Ran out of mana!");
                    return;
                }

                holder.TriggerActionEffect(manaCost, caster, target.transform.position);
            }
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void WhenPutOnADSStack(CombatListener input, ActionEffectBase output)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} will react with {nameKey}!");

        hasTriggered = false;
    }
}
