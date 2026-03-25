using System;
using System.Diagnostics;
using UnityEngine;

public class ICombatObject_Buff : ICombatObject
{

    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected BuffORDebuffBase buffEffect;

    protected DroneUnitBody target;

    public float myDamageType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public event Action<ICombatObject> MyActionDelegate;
    protected ICombatDelegateTriggers myDelegateTriggerType = ICombatDelegateTriggers.NONE;

    public ICombatDelegateTriggers MyDelegateTriggerType => myDelegateTriggerType;

    public DroneUnitBody RespondActionTarget { get; set; }
    public float RespondActionMana { get; set; }

    public virtual void SetupBuff(BuffsEnum targetBuff)
    {
        switch (targetBuff)
        {
            case BuffsEnum.None:
                break;
            case BuffsEnum.Overdrive: 
                buffEffect = new OverdriveBuff();
                break;
            case BuffsEnum.MartialProwess:
                buffEffect = new MartialProwessBuff();
                break;
            case BuffsEnum.MagicalProwess:
                buffEffect = new MagicalProwessBuff();
                break;
            case BuffsEnum.ArmorPolish:
                buffEffect = new ArmorPolishBuff();
                break;
            case BuffsEnum.ManaReinforcement:
                buffEffect = new ManaReinforcementBuff();
                break;
            case BuffsEnum.CriticalProtection:
                buffEffect = new CriticalProtectionBuff();
                break;
            case BuffsEnum.Multihit:
                buffEffect = new MultiHitsBuff();
                break;
            case BuffsEnum.ManaRegeneration:
                buffEffect = new ManaRegenerationBuff();
                break;
            case BuffsEnum.HealthRegeneration:
                buffEffect = new HealthRegenerationBuff();
                break;
            case BuffsEnum.StatusProtection:
                buffEffect = new StatusProtectionBuff();
                break;
        }

        buffEffect.InitBuffDebuff(this);
    }

    public void CombatUpdate()
    {
        bool hasMana = buffEffect.BuffDebuffDuration();

        if (hasMana == false)
        {
            if (myDelegateTriggerType == ICombatDelegateTriggers.ON_FINISHED) TriggerDelegate();
            isActive = false;
        }
    }

    public bool FinalEffectReturnValue()
    {
        throw new NotImplementedException();
    }

    public bool FinalEffectReturnValue(Vector3 triggerPos)
    {
        throw new NotImplementedException();
    }

    public bool FinalEffectReturnValue(DroneUnitBody triggeredDrone)
    {
        throw new NotImplementedException();
    }

    public bool FinalEffectReturnValue(GameObject triggeredObject)
    {
        throw new NotImplementedException();
    }

    public void MyRespondAction(ICombatObject obj)
    {
        Reactivate(obj.RespondActionMana, obj.RespondActionTarget);
    }

    public virtual void OnSpawn(DroneUnitBody caster, ActionEffectBase origin, ICombatDelegateTriggers delegateTrigger)
    {
        myCaster = caster;
        myOrigin = origin;
        myDelegateTriggerType = delegateTrigger;
    }

    public void Reactivate(float mana)
    {
        throw new NotImplementedException();
    }

    public void Reactivate(float mana, Vector3 targetPos)
    {
        throw new NotImplementedException();
    }

    public void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        isActive = true;
        target = otherCaster;
        buffEffect.AttachBuffDebuff(mana, target);
        if (myDelegateTriggerType == ICombatDelegateTriggers.ON_REACTIVATE) TriggerDelegate();
    }

    public void Reactivate(float mana, GameObject targetObj)
    {
        throw new NotImplementedException();
    }

    public void TriggerDelegate()
    {
        MyActionDelegate?.Invoke(this);
    }
}
