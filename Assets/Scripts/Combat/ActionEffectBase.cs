using System;
using UnityEngine;


public enum effectType//This is used to chose what child class of ActionEffectBase to create!
{
    NONE, ShotProjectile, Move,
}


public abstract class ActionEffectBase
{
    public abstract void TriggerActionEffect(float mana,DroneUnitBody caster);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody otherCaster);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody[] otherCasters);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject targetObj);
    public abstract void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject[] targetObjs);
}


