using UnityEngine;
using actions;
using System;
using System.Runtime.CompilerServices;

public class ReactionNode_QuickThrow : ActionNodeBase
{
    public void SetProjectilesPath(String projectilePath)
    {
        projectileStringPath = projectilePath;

        if (effect is AE_ShotProjectile)
        {
            AE_ShotProjectile s = effect as AE_ShotProjectile;
            s.SetAssetPath(projectileStringPath);
        }
    }

    private string projectileStringPath;

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
        float score = caster.MyHP <= 0 ? 0f : UnityEngine.Random.Range(20, 80);

        if (caster.MyHP <= 0) score = -1;
        
        return score;
        
    }

    public override void HandleADSOutputChain(Func<ActionEffectBase> ancestralOutputChain, int maxChainIndex, int chainIndex)
    {
        foreach (Func<ActionEffectBase> func in ancestralOutputChain.GetInvocationList())
        {
            AE_ShotProjectile aE_ShotProjectile = (AE_ShotProjectile)func();

            if (aE_ShotProjectile != null)
            {
                DroneUnitBody target = CombatListener.GetClosesTarget(caster.IsEnemy, caster.transform.position);

                if (target == null) continue;

                caster.ManaSpent(manaCost);

                if (caster.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Ran out of mana!");
                    return;
                }

                aE_ShotProjectile.TriggerActionEffect(manaCost, caster, target.transform.position);
            }
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void WhenPutOnADSStack(CombatListener input, ActionEffectBase output)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} will react with QuickThrow!");

        hasTriggered = false;
    }
}
