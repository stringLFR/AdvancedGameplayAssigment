using System;
using UnityEngine;

public class ICombaatObject_Melee : ICombatObject
{
    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected Melee prefab;

    protected Vector3 target;

    public float myDamageType {
        get
        {
            if (prefab.isMagic == true) return Caster.MyMelee_M_HitRate + Caster.MagicalProwess - Caster.MagicalIneptitiude;
            else return Caster.MyMelee_P_HitRate + Caster.MartialProwess - Caster.MartialIneptitiude;
        }
        set { }
    }

    public ICombaatObject_Melee(GameObject path, Vector3 pos)
    {
        SetupMeleeObj(path, pos);
    }

    protected virtual void SetupMeleeObj(GameObject path, Vector3 pos)
    {
        GameObject obj = UnityEngine.Object.Instantiate(path, pos, Quaternion.identity);
        obj.SetActive(false);
        obj.transform.parent = Combat.instanceTransfrom;
        prefab = obj.GetComponent<Melee>();
        prefab.InitMelee(this);
    }

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public event Action<ICombatObject> MyActionDelegate;

    public virtual void CombatUpdate()
    {
        bool hasMana = prefab.Swinging();

        if (hasMana == false)
        {
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
            TriggerDelegate();

            return true;
        }
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

    public virtual void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
    }

    public virtual void Reactivate(float mana)
    {
        isActive = true;
        prefab.gameObject.SetActive(true);
        prefab.UnSheath(mana);
    }

    public void Reactivate(float mana, Vector3 targetPos)
    {
        throw new NotImplementedException();
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
}
