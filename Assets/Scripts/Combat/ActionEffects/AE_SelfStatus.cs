using System.Collections.Generic;
using UnityEngine;

public class AE_SelfStatus : ActionEffectBase
{
    protected ICombatObject_Status status;

    private StatusEnum myStatusType;

    public virtual void SetStatusType(StatusEnum path)
    {
        myStatusType = path;
    }

    protected virtual void SetUpStatus(DroneUnitBody caster, float mana)
    {
        if (status != null && status.IsActive == false)
        {
            //d.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

            status.Reactivate(mana, caster);

            Combat.actionEffectObjects.Add(status);

            return;
        }

        status = new ICombatObject_Status();
        status.SetupStatus(myStatusType);

        status.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        status.Reactivate(mana, caster);

        Combat.actionEffectObjects.Add(status);
    }

    public override void DelegateHandler(ICombatObject Iobj)
    {
        throw new System.NotImplementedException();
    }

    public override void SetAssetPath(string path)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        SetUpStatus(caster, mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        SetUpStatus(caster, mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody[] otherCasters)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject[] targetObjs)
    {
        throw new System.NotImplementedException();
    }
}
