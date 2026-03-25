using System;
using UnityEngine;

public class ICombatObject_SummonObject : ICombatObject
{

    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected SummonObject prefab;

    protected Vector3 target;
    protected ICombatDelegateTriggers myDelegateTriggerType = ICombatDelegateTriggers.NONE;

    public ICombatDelegateTriggers MyDelegateTriggerType => myDelegateTriggerType;
    public float myDamageType {
        get
        {
            if (prefab.isMagic == true) return Caster.MyRanged_M_HitRate + Caster.MagicalProwess - Caster.MagicalIneptitiude;
            else return Caster.MyRanged_P_HitRate + Caster.MartialProwess - Caster.MartialIneptitiude;
        }
        set { }
    }

    public ICombatObject_SummonObject(GameObject path, Vector3 pos)
    {
        SetupSummonObj(path, pos);
    }

    protected virtual void SetupSummonObj(GameObject path, Vector3 pos)
    {
        GameObject obj = UnityEngine.Object.Instantiate(path, pos, Quaternion.identity);
        obj.SetActive(false);
        obj.transform.parent = Combat.instanceTransfrom;
        prefab = obj.GetComponent<SummonObject>();
        prefab.InitSummonedObject(this);
    }

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public DroneUnitBody RespondActionTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float RespondActionMana { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action<ICombatObject> MyActionDelegate;

    public virtual void CombatUpdate()
    {
        if (prefab == null)
        {
            isActive = false;
            return;
        }

        bool hasMana = prefab.SustainSummon(target);

        if (hasMana == false)
        {
            if (myDelegateTriggerType == ICombatDelegateTriggers.ON_FINISHED) TriggerDelegate();
            prefab.gameObject.SetActive(false);
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

    public virtual bool FinalEffectReturnValue(DroneUnitBody triggeredDrone)
    {
        if (triggeredDrone != Caster)
        {
            if (myDelegateTriggerType == ICombatDelegateTriggers.ON_DRONEHIT)
            {
                RespondActionTarget = triggeredDrone;
                TriggerDelegate();
            }

            return true;
        }
        RespondActionTarget = null;

        return false;
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

    public virtual void Reactivate(float mana, Vector3 targetPos)
    {
        isActive = true;
        target = targetPos;
        prefab.gameObject.SetActive(true);
        prefab.Summon(mana, myCaster.transform.position, myCaster.transform.position - target);
        if (myDelegateTriggerType == ICombatDelegateTriggers.ON_REACTIVATE) TriggerDelegate();
    }

    public void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        throw new NotImplementedException();
    }

    public void Reactivate(float mana, GameObject targetObj)
    {
        throw new NotImplementedException();
    }

    public void TriggerDelegate()
    {
        MyActionDelegate?.Invoke(this);
    }

    public void MyRespondAction(ICombatObject obj)
    {
        Reactivate(obj.RespondActionMana, obj.RespondActionTarget.transform.position);
    }
}
