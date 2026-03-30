using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_SelfBuff : ActionEffectBase
{

    protected List<ICombatObject_Buff> buffs = new List<ICombatObject_Buff>();

    private BuffsEnum myBuffType;

    public virtual void SetBuffType(BuffsEnum path)
    {
        myBuffType = path;
    }

    protected virtual void SetUpBuff(DroneUnitBody caster, float mana)
    {
        ICombatObject_Buff b = buffs.Find(p => p.IsActive == false);

        if (b != null)
        {
            //b.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

            b.Reactivate(mana, caster);

            Combat.actionEffectObjects.Add(b);

            return;
        }

        ICombatObject_Buff bNew = new ICombatObject_Buff();
        bNew.SetupBuff(myBuffType);
        buffs.Add(bNew);

        bNew.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        bNew.Reactivate(mana, caster);

        Combat.actionEffectObjects.Add(bNew);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        SetUpBuff(caster, mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        SetUpBuff(caster, mana);
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
