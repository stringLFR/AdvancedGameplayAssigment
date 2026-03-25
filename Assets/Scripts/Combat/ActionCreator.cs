using actions;
using UnityEngine;

public enum NodeType //This is used to chose what child class of ActionNodeBase to create!
{
    NONE, RockThrow, QuickSlash,QuickSplash,
}

public enum MainActionTypes//This is used to chose what child class of MainActionBase to create!
{
    NONE, RunToPoint,
}

public static class ActionCreator
{
    public static MainActionBase CreateMainAction(MainActionStats stats, string userName)
    {
        switch (stats.mainActionType)
        {
            case MainActionTypes.RunToPoint:

                MainAction_RunToPoint runToPoint = new MainAction_RunToPoint(CreateActionEffect(stats.Effect),stats.MainActionName + " " + userName, stats.MainActionDescription ,stats.manaCost, stats.actionType, stats.Effect);

                return runToPoint;
        }
        return null;
    }

    public static ActionNodeBase CreateReactionAction(ActionNodeStats stats, string userName)
    {
        switch (stats.Node)
        {
            case NodeType.NONE:
                break;
            case NodeType.RockThrow:

                ReactionNode_QuickThrow quickThrow = new ReactionNode_QuickThrow();
                quickThrow.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions);
                quickThrow.SetQuickThrowPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");
                return quickThrow;

            case NodeType.QuickSlash:
                ReactionNode_QuickThrow quickSlash = new ReactionNode_QuickThrow();
                quickSlash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions);
                quickSlash.SetQuickThrowPrefabPath("Assets/Prefabs/QuckSlash.prefab");
                return quickSlash;
            case NodeType.QuickSplash:
                ReactionNode_QuickThrow quickSplash = new ReactionNode_QuickThrow();
                quickSplash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions);
                quickSplash.SetQuickThrowPrefabPath("Assets/Prefabs/Areas/QuickSplash.prefab");
                return quickSplash;
        }

        return null;
    }

    private static ActionEffectBase CreateActionEffect(effectType effect)
    {
        switch (effect)
        {
            case effectType.NONE:
                break;
            case effectType.Move:

                AE_Move move = new AE_Move();

                return move;

            case effectType.ShotProjectile:

                AE_ShotProjectile projectile = new AE_ShotProjectile();

                return projectile;
            case effectType.SummonPbject:

                AE_SummonObject summon = new AE_SummonObject();

                return summon;
            case effectType.CreateArea:

                AE_CreateArea area = new AE_CreateArea();

                return area;
            case effectType.SwingMelee:

                AE_SwingMelee melee = new AE_SwingMelee();

                return melee;
            case effectType.KnockBackCombo:

                AE_KnockBackCombo knockBackCombo = new AE_KnockBackCombo();

                return knockBackCombo;
            case effectType.PercingCombo:

                AE_PercingCombo aE_PercingCombo = new AE_PercingCombo();

                return aE_PercingCombo;
            case effectType.StunningCombo:

                AE_StunningCombo aE_StunningCombo = new AE_StunningCombo();

                return aE_StunningCombo;
            case effectType.HackingCombo:

                AE_HackingCombo aE_HackingCombo = new AE_HackingCombo();

                return aE_HackingCombo;
        }

        return null;
    }
}
