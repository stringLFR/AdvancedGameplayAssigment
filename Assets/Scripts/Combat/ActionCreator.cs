using actions;
using UnityEngine;

public enum NodeType //This is used to chose what child class of ActionNodeBase to create!
{
    NONE, QuickShoot, QuickSlash,QuickSplash,
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
            case MainActionTypes.NONE:
                break;
            case MainActionTypes.RunToPoint:

                MainAction_TargetPoint runToPoint = new MainAction_TargetPoint(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName, stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);

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
            case NodeType.QuickShoot:

                ReactionNode_QuickThrow quickThrow = new ReactionNode_QuickThrow();
                quickThrow.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions,stats.IsTeamworkAction);
                quickThrow.SetQuickThrowPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");
                return quickThrow;
            case NodeType.QuickSlash:
                ReactionNode_QuickThrow quickSlash = new ReactionNode_QuickThrow();
                quickSlash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions, stats.IsTeamworkAction);
                quickSlash.SetQuickThrowPrefabPath("Assets/Prefabs/QuckSlash.prefab");
                return quickSlash;
            case NodeType.QuickSplash:
                ReactionNode_QuickThrow quickSplash = new ReactionNode_QuickThrow();
                quickSplash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions, stats.IsTeamworkAction);
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
            case effectType.ManaburnCombo:

                AE_ManaburnCombo aE_ManaburnCombo = new AE_ManaburnCombo();

                return aE_ManaburnCombo;
            case effectType.ArmorBreakCombo:

                AE_ArmorBreakCombo _ArmorBreakCombo = new AE_ArmorBreakCombo();

                return _ArmorBreakCombo;
            case effectType.ManaSusceptibilityCombo:

                AE_ManaSusceptibilityCombo _ManaSusceptibilityCombo = new AE_ManaSusceptibilityCombo();

                return _ManaSusceptibilityCombo;
            case effectType.CriticalVulnerabilityCombo:

                AE_CriticalVulnerabilityCombo aE_CriticalVulnerabilityCombo = new AE_CriticalVulnerabilityCombo();

                return aE_CriticalVulnerabilityCombo;
            case effectType.StatusVulnerabilityCombo:

                AE_StatusVulnerabilityCombo aE_StatusVulnerabilityCombo = new AE_StatusVulnerabilityCombo();

                return aE_StatusVulnerabilityCombo;
            case effectType.CriticalExploitCombo:

                AE_CriticalExploitCombo _CriticalExploitCombo = new AE_CriticalExploitCombo();

                return _CriticalExploitCombo;
            case effectType.IneptitiudeCombo:

                AE_IneptitiudeCombo aE_IneptitiudeCombo = new AE_IneptitiudeCombo();

                return aE_IneptitiudeCombo;
            case effectType.DrainCombo:

                AE_DrainCombo aE_DrainCombo = new AE_DrainCombo();

                return aE_DrainCombo;
            case effectType.ProwessCombo:

                AE_ProwessCombo aE_ProwessCombo = new AE_ProwessCombo();

                return aE_ProwessCombo;
            case effectType.ReapingCombo:

                AE_ReapingCombo aE_ReapingCombo = new AE_ReapingCombo();

                return aE_ReapingCombo;
            case effectType.DefenciveCombo:

                AE_DefenciveCombo aE_DefenciveCombo = new AE_DefenciveCombo();

                return aE_DefenciveCombo;
            case effectType.AnitVulnerabilityCombo:

                AE_AnitVulnerabilityCombo e_AnitVulnerabilityCombo = new AE_AnitVulnerabilityCombo();

                return e_AnitVulnerabilityCombo;
            case effectType.NegationCombo:

                AE_NegationCombo aE_NegationCombo = new AE_NegationCombo();

                return aE_NegationCombo;
            case effectType.KnockBackZone:

                AE_KnockBackZone aE_KnockBackZone = new AE_KnockBackZone();

                return aE_KnockBackZone;
            case effectType.ShrapnelZone:
                break;
            case effectType.HackingZone:
                break;
            case effectType.ManaBurnZone:
                break;
            case effectType.NegationZone:
                break;
            case effectType.StunningZone:
                break;
            case effectType.ArmorDownZone:
                break;
            case effectType.ManaShieldDownZone:
                break;
            case effectType.CritInflictZone:
                break;
            case effectType.StatusInflictZone:
                break;
            case effectType.MartialDownZone:
                break;
            case effectType.MagicalDownZone:
                break;
            case effectType.ManaDownZone:
                break;
            case effectType.DecayZone:
                break;
            case effectType.MartialUpZone:
                break;
            case effectType.MagicalUpZone:
                break;
            case effectType.ArmorUpZone:
                break;
            case effectType.ManaShieldUpZone:
                break;
            case effectType.CriticalProtectionZone:
                break;
            case effectType.StatusProtectionZone:
                break;
            case effectType.ManaUpZone:
                break;
            case effectType.HealthUpZone:
                break;
            case effectType.KnockBackShoot:
                break;
            case effectType.PiercingShoot:
                break;
            case effectType.HackShoot:
                break;
            case effectType.ManaBurnShoot:
                break;
            case effectType.ArmorBreakShoot:
                break;
            case effectType.ManaSusceptibilityShoot:
                break;
            case effectType.CriticalVulnerabilityShoot:
                break;
            case effectType.StatusVulnerabilityShoot:
                break;
            case effectType.CriticalExploitShoot:
                break;
            case effectType.IneptitiudeShoot:
                break;
            case effectType.DrainShoot:
                break;
            case effectType.ProwessShoot:
                break;
            case effectType.ReapingShoot:
                break;
            case effectType.DefenciveShoot:
                break;
            case effectType.AnitVulnerabilityShoot:
                break;
            case effectType.NegationShoot:
                break;
        }

        return null;
    }
}
