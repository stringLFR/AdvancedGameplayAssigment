using System;
using UnityEngine;

public class ICombatObject_Status : ICombatObject
{
    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected StatusBase satusEffect;

    protected DroneUnitBody target;


    public float myDamageType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public event Action<ICombatObject> MyActionDelegate;

    protected ICombatDelegateTriggers myDelegateTriggerType = ICombatDelegateTriggers.NONE;

    public ICombatDelegateTriggers MyDelegateTriggerType => myDelegateTriggerType;
    protected virtual void SetupStatus(StatusEnum targetStatus)
    {
        switch (targetStatus)
        {
            case StatusEnum.None:
                break;
            case StatusEnum.Stunned:
                satusEffect = new Status_Stunned();
                break;
            case StatusEnum.Kncokback:
                satusEffect = new Status_Knockback();
                break;
            case StatusEnum.Leaking:
                satusEffect = new Status_Leaking();
                break;
            case StatusEnum.Negation:
                satusEffect = new Status_Negation();
                break;
            case StatusEnum.Hacked:
                satusEffect = new Status_Hacked();
                break;
        }

        satusEffect.InitStatus(this);
    }

    public void CombatUpdate()
    {
        bool hasMana = satusEffect.TriggerStatus();

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

    public void MyRespondAction(ICombatObject obj, Vector3 targetPos, DroneUnitBody otherCaster = null, GameObject triggeredObject = null)
    {
        throw new NotImplementedException();
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
        satusEffect.AttachStatus(mana, otherCaster);
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
