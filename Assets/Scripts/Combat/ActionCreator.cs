using actions;
using UnityEngine;

public enum NodeType //This is used to chose what child class of ActionNodeBase to create!
{
    NONE, QuickShoot, QuickSlash,QuickSplash, 
}

public enum MainActionTypes//This is used to chose what child class of MainActionBase to create!
{
    NONE, RunToPoint, ShootToPoint, AreaToPoint, MeleeToPoint, ShootsToPoints, AreasToPoints, strikesToPoints, areaAtCaster, shootAtCaster,meleeAtCaster,
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

                MainAction_TargetPoint runToPoint = new MainAction_TargetPoint(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);

                return runToPoint;
            case MainActionTypes.ShootToPoint:

                MainAction_TargetPoint shootToPoint = new MainAction_TargetPoint(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                shootToPoint.SetTargetPointPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");

                return shootToPoint;
            case MainActionTypes.AreaToPoint:

                MainAction_TargetPoint areaToPoint = new MainAction_TargetPoint(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                areaToPoint.SetTargetPointPrefabPath("Assets/Prefabs/Areas/QuickSplash.prefab");

                return areaToPoint;
            case MainActionTypes.MeleeToPoint:

                MainAction_TargetPoint meleeToPoint = new MainAction_TargetPoint(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                meleeToPoint.SetTargetPointPrefabPath("Assets/Prefabs/QuckSlash.prefab");

                return meleeToPoint;
            case MainActionTypes.strikesToPoints:

                MainAction_TargetManyPoints meleeToPoints = new MainAction_TargetManyPoints(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                meleeToPoints.SetTargetManyPrefabPath("Assets/Prefabs/QuckSlash.prefab");
                meleeToPoints.Init(10f);

                return meleeToPoints;
            case MainActionTypes.AreasToPoints:

                MainAction_TargetManyPoints areaToPoints = new MainAction_TargetManyPoints(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                areaToPoints.SetTargetManyPrefabPath("Assets/Prefabs/Areas/QuickSplash.prefab");
                areaToPoints.Init(10f);

                break;
            case MainActionTypes.ShootsToPoints:

                MainAction_TargetManyPoints shootsToPoints = new MainAction_TargetManyPoints(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                shootsToPoints.SetTargetManyPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");
                shootsToPoints.Init(10f);

                return shootsToPoints;
            case MainActionTypes.areaAtCaster:

                MainAction_TargetSelf areaAtSelf = new MainAction_TargetSelf(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                areaAtSelf.SetTargetSelfPrefabPath("Assets/Prefabs/Areas/QuickSplash.prefab");

                return areaAtSelf;
            case MainActionTypes.shootAtCaster:

                MainAction_TargetSelf shootAtSelf = new MainAction_TargetSelf(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                shootAtSelf.SetTargetSelfPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");

                return shootAtSelf;
            case MainActionTypes.meleeAtCaster:

                MainAction_TargetSelf meleeAtSelf = new MainAction_TargetSelf(CreateActionEffect(stats.Effect), stats.MainActionName + " " + userName,
                    stats.MainActionDescription, stats.manaCost, stats.actionType, stats.Effect);
                meleeAtSelf.SetTargetSelfPrefabPath("Assets/Prefabs/QuckSlash.prefab");

                return meleeAtSelf;
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
                quickThrow.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), 
                    stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions,stats.IsTeamworkAction, stats.TargetAlly);
                quickThrow.SetQuickThrowPrefabPath("Assets/Prefabs/Projectiles/Rock.prefab");
                return quickThrow;
            case NodeType.QuickSlash:
                ReactionNode_QuickThrow quickSlash = new ReactionNode_QuickThrow();
                quickSlash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), 
                    stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions, stats.IsTeamworkAction, stats.TargetAlly);
                quickSlash.SetQuickThrowPrefabPath("Assets/Prefabs/QuckSlash.prefab");
                return quickSlash;
            case NodeType.QuickSplash:
                ReactionNode_QuickThrow quickSplash = new ReactionNode_QuickThrow();
                quickSplash.Init(stats.NodeName + " " + userName, stats.NodeInfo, stats.IsRoot, CreateActionEffect(stats.Effect), 
                    stats.MinScore, stats.ManaCost, stats.ActionType, stats.Reactions, stats.IsTeamworkAction, stats.TargetAlly);
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

                AE_ShrapnelZone aE_ShrapnelZone = new AE_ShrapnelZone();

                return aE_ShrapnelZone;
            case effectType.HackingZone:

                AE_HackingZone aE_HackingZone = new AE_HackingZone();

                return aE_HackingZone;
            case effectType.ManaBurnZone:

                AE_ManaBurnZone aE_ManaBurnZone = new AE_ManaBurnZone();

                return aE_ManaBurnZone;
            case effectType.NegationZone:

                AE_NegationZone aE_NegationZone = new AE_NegationZone();

                return aE_NegationZone;
            case effectType.StunningZone:

                AE_StunningZone aE_StunningZone = new AE_StunningZone();

                return aE_StunningZone;
            case effectType.ArmorDownZone:

                AE_ArmorDownZone aE_ArmorDownZone = new AE_ArmorDownZone();

                return aE_ArmorDownZone;
            case effectType.ManaShieldDownZone:

                AE_ManaShieldDownZone aE_ManaShieldDownZone = new AE_ManaShieldDownZone();

                return aE_ManaShieldDownZone;
            case effectType.CritInflictZone:

                AE_CritInflictZone aE_CritInflictZone = new AE_CritInflictZone();

                return aE_CritInflictZone;
            case effectType.StatusInflictZone:

                AE_StatusInflictZone aE_StatusInflictZones = new AE_StatusInflictZone();

                return aE_StatusInflictZones;
            case effectType.MartialDownZone:

                AE_MartialDownZone aE_MartialDownZone = new AE_MartialDownZone();

                return aE_MartialDownZone;
            case effectType.MagicalDownZone:

                AE_MagicalDownZone aE_MagicalDownZone = new AE_MagicalDownZone();

                return aE_MagicalDownZone;
            case effectType.ManaDownZone:

                AE_ManaDownZone aE_ManaDownZone = new AE_ManaDownZone();

                return aE_ManaDownZone;
            case effectType.DecayZone:

                AE_DecayZone aE_DecayZone = new AE_DecayZone(); 

                return aE_DecayZone;
            case effectType.MartialUpZone:

                AE_MartialUpZone aE_MartialUpZone = new AE_MartialUpZone();

                return aE_MartialUpZone;
            case effectType.MagicalUpZone:

                AE_MagicalUpZone aE_MagicalUpZone = new AE_MagicalUpZone();

                return aE_MagicalUpZone;
            case effectType.ArmorUpZone:

                AE_ArmorUpZone aE_ArmorUpZone = new AE_ArmorUpZone();

                return aE_ArmorUpZone;
            case effectType.ManaShieldUpZone:

                AE_ManaShieldUpZone aE_ManaShieldUpZone = new AE_ManaShieldUpZone();

                return aE_ManaShieldUpZone;
            case effectType.CriticalProtectionZone:

                AE_CriticalProtectionZone aE_CriticalProtectionZone = new AE_CriticalProtectionZone();

                return aE_CriticalProtectionZone;
            case effectType.StatusProtectionZone:

                AE_StatusProtectionZone aE_StatusProtectionZone = new AE_StatusProtectionZone();

                return aE_StatusProtectionZone;
            case effectType.ManaUpZone:

                AE_ManaUpZone aE_ManaUpZone = new AE_ManaUpZone();

                return aE_ManaUpZone;
            case effectType.HealthUpZone:

                AE_HealthUpZone aE_HealthUpZone = new AE_HealthUpZone();

                return aE_HealthUpZone;
            case effectType.KnockBackShoot:

                AE_KnockBackShoot aE_KnockBackShoot = new AE_KnockBackShoot();

                return aE_KnockBackShoot;
            case effectType.PiercingShoot:

                AE_PiercingShoot aE_PiercingShoot = new AE_PiercingShoot();

                return aE_PiercingShoot;
            case effectType.HackShoot:

                AE_HackShoot aE_HackShoot = new AE_HackShoot();

                return aE_HackShoot;
            case effectType.ManaBurnShoot:

                AE_ManaBurnShoot aE_ManaBurnShoot = new AE_ManaBurnShoot();

                return aE_ManaBurnShoot;
            case effectType.ArmorBreakShoot:

                AE_ArmorBreakShoot aE_ArmorBreakShoot = new AE_ArmorBreakShoot();

                return aE_ArmorBreakShoot;
            case effectType.ManaSusceptibilityShoot:

                AE_ManaSusceptibilityShoot aE_ManaSusceptibilityShoot = new AE_ManaSusceptibilityShoot();

                return aE_ManaSusceptibilityShoot;
            case effectType.CriticalVulnerabilityShoot:

                AE_CriticalVulnerabilityShoot aE_CriticalVulnerabilityShoot = new AE_CriticalVulnerabilityShoot();

                return aE_CriticalVulnerabilityShoot;
            case effectType.StatusVulnerabilityShoot:

                AE_StatusVulnerabilityShoot aE_StatusVulnerabilityShoot = new AE_StatusVulnerabilityShoot();

                return aE_StatusVulnerabilityShoot;
            case effectType.CriticalExploitShoot:

                AE_CriticalExploitShoot aE_CriticalExploitShoot = new AE_CriticalExploitShoot();

                return aE_CriticalExploitShoot;
            case effectType.IneptitiudeShoot:

                AE_IneptitiudeShoot aE_IneptitiudeShoot = new AE_IneptitiudeShoot();

                return aE_IneptitiudeShoot;
            case effectType.DrainShoot:

                AE_DrainShoot aE_DrainShoot = new AE_DrainShoot();

                return aE_DrainShoot;
            case effectType.ProwessShoot:

                AE_ProwessShoot aE_ProwessShoot = new AE_ProwessShoot();

                return aE_ProwessShoot;
            case effectType.ReapingShoot:

                AE_ReapingShoot aE_ReapingShoot = new AE_ReapingShoot();

                return aE_ReapingShoot;
            case effectType.DefenciveShoot:

                AE_DefenciveShoot aE_DefenciveShoot = new AE_DefenciveShoot();

                return aE_DefenciveShoot;
            case effectType.AnitVulnerabilityShoot:

                AE_AnitVulnerabilityShoot aE_AnitVulnerabilityShoot = new AE_AnitVulnerabilityShoot();

                return aE_AnitVulnerabilityShoot;
            case effectType.NegationShoot:

                AE_NegationShoot aE_NegationShoot = new AE_NegationShoot();

                return aE_NegationShoot;
        }

        return null;
    }
}
