using actions;
using UnityEngine;

public static class ActionCreator
{
    public static MainActionBase CreateMainAction(MainActionStats stats)
    {
        switch (stats.mainActionType)
        {
            case MainActionTypes.RunToPoint:

                MainAction_RunToPoint runToPoint = new MainAction_RunToPoint(CreateActionEffect(stats.Effect),stats.MainActionName, stats.manaCost, stats.actionType, stats.Effect);

                return runToPoint;
        }
        return null;
    }

    public static ActionNodeBase CreateReactionAction(ActionNodeStats stats)
    {
        switch (stats.Node)
        {
            case NodeType.QuickThrow:

                ReactionNode_QuickThrow quickThrow = new ReactionNode_QuickThrow();
                quickThrow.Init(stats.NodeName, stats.IsRoot, CreateActionEffect(stats.Effect),stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions);
                quickThrow.SetProjectilesPath("Assets/Prefabs/Projectiles/Rock.prefab");
                return quickThrow;
        }

        return null;
    }

    private static ActionEffectBase CreateActionEffect(effectType effect)
    {
        switch (effect)
        {
            case effectType.Move: 
                
                AE_Move move = new AE_Move();
                
                return move;

            case effectType.ShotProjectile:

                AE_ShotProjectile projectile = new AE_ShotProjectile();

                return projectile;
        }

        return null;
    }
}
