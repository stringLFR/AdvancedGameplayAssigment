using actions;
using UnityEngine;

public enum NodeType //This is used to chose what child class of ActionNodeBase to create!
{
    NONE, RockThrow, QuickSlash,
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
                quickThrow.SetQuickTrhowPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");
                return quickThrow;
            
            case NodeType.QuickSlash:
                ReactionNode_QuickThrow quickSlash = new ReactionNode_QuickThrow();
                quickSlash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions);
                quickSlash.SetQuickTrhowPrefabPath("Assets/Prefabs/QuckSlash.prefab");
                return quickSlash;
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
        }

        return null;
    }
}
