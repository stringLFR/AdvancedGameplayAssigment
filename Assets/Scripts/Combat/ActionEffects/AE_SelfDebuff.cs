using System.Collections.Generic;
using UnityEngine;

public class AE_SelfDebuff : ActionEffectBase
{
    protected List<ICombatObject_Debuff> debuffs = new List<ICombatObject_Debuff>();

    private DebuffsEnum myDebuffType;

    public virtual void SetDebuffType(DebuffsEnum path)
    {
        myDebuffType = path;
    }

    protected virtual void SetUpDebuff(DroneUnitBody caster, float mana)
    {
        ICombatObject_Debuff d = debuffs.Find(p => p.IsActive == false);

        if (d != null)
        {
            //d.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

            d.Reactivate(mana);

            Combat.actionEffectObjects.Add(d);

            return;
        }

        ICombatObject_Debuff dNew = new ICombatObject_Debuff();
        dNew.SetupDebuff(myDebuffType);
        debuffs.Add(dNew);

        dNew.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        dNew.Reactivate(mana);

        Combat.actionEffectObjects.Add(dNew);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        SetUpDebuff(caster, mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        throw new System.NotImplementedException();
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

    public override void SetAssetPath(string path)
    {
        throw new System.NotImplementedException();
    }

    public override void DelegateHandler(ICombatObject Iobj)
    {
        throw new System.NotImplementedException();
    }
}
