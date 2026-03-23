using System;
using UnityEngine;

public class ICombatObject_Debuff : ICombatObject
{

    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected BuffORDebuffBase debuffEffect;

    protected DroneUnitBody target;

    public float myDamageType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public event Action<ICombatObject> MyActionDelegate;


    protected virtual void SetupDebuff(DebuffsEnum targetDebuff)
    {
        switch (targetDebuff)
        {
            case DebuffsEnum.None:
                break;
            case DebuffsEnum.ArmorBreak:
                debuffEffect = new ArmorBreakDebuff();
                break;
            case DebuffsEnum.ManaSusceptibility:
                debuffEffect = new ManaSusceptibilityDebuff();
                break;
            case DebuffsEnum.MartialIneptitiude:
                debuffEffect = new MartialIneptitiudeDebuff();
                break;
            case DebuffsEnum.MagicalIneptitiude:
                debuffEffect = new MagicalIneptitiudeDebuff();
                break;
            case DebuffsEnum.CriticalVulnerability:
                debuffEffect = new CriticalVulnerabilityDebuff();
                break;
            case DebuffsEnum.CriticalExploit:
                debuffEffect = new CriticalExploitDebuff();
                break;
            case DebuffsEnum.StatusVulnerability:
                debuffEffect = new StatusVulnerabilityDebuff();
                break;
            case DebuffsEnum.HealthDrain:
                debuffEffect = new HealthDrainDebuff();
                break;
            case DebuffsEnum.ManaDrain:
                debuffEffect = new ManaDrainDebuff();
                break;
        }

        debuffEffect.InitBuffDebuff(this);
    }

    public void CombatUpdate()
    {
        bool hasMana = debuffEffect.BuffDebuffDuration();

        if (hasMana == false)
        {
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

    public void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
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
        debuffEffect.AttachBuffDebuff(mana, target);
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
