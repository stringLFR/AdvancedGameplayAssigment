using System;
using UnityEngine;


public enum effectType//This is used to chose what child class of ActionEffectBase to create!
{
    //General
    NONE, ShotProjectile, Move, SummonPbject, CreateArea, SwingMelee,

    //Melee modified
    KnockBackCombo, PercingCombo, StunningCombo, HackingCombo, ManaburnCombo, ArmorBreakCombo, 
    ManaSusceptibilityCombo, CriticalVulnerabilityCombo, StatusVulnerabilityCombo, CriticalExploitCombo, IneptitiudeCombo, DrainCombo, ProwessCombo, ReapingCombo, 
    DefenciveCombo, AnitVulnerabilityCombo, NegationCombo,

    //Area modified
    KnockBackZone, ShrapnelZone, HackingZone, ManaBurnZone, NegationZone, StunningZone, ArmorDownZone, ManaShieldDownZone, CritInflictZone, StatusInflictZone,
    MartialDownZone, MagicalDownZone, ManaDownZone, DecayZone, MartialUpZone, MagicalUpZone, ArmorUpZone, ManaShieldUpZone, CriticalProtectionZone,
    StatusProtectionZone, ManaUpZone, HealthUpZone,

    //projectile modified
    KnockBackShoot, PiercingShoot, HackShoot, ManaBurnShoot, ArmorBreakShoot, ManaSusceptibilityShoot, CriticalVulnerabilityShoot, StatusVulnerabilityShoot,
    CriticalExploitShoot, IneptitiudeShoot, DrainShoot, ProwessShoot, ReapingShoot, DefenciveShoot, AnitVulnerabilityShoot, NegationShoot,
}


public abstract class ActionEffectBase
{
    public DroneUnitBody TeamWorkTarget { get; set; }
    public abstract void DelegateHandler(ICombatObject Iobj);
    public abstract void SetAssetPath(string path);
    public abstract void TriggerActionEffect(float mana,DroneUnitBody caster);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody otherCaster);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody[] otherCasters);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject targetObj);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject[] targetObjs);
}


