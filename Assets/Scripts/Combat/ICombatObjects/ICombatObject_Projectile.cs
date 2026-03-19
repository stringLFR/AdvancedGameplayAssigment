using System.Runtime.CompilerServices;
using UnityEngine;

public class ICombatObject_Projectile : ICombatObject
{
    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected Projectile prefab;

    protected Vector3 target;

    public event System.Action<ICombatObject> MyActionDelegate;

    public ICombatObject_Projectile(GameObject path, Vector3 pos)
    {
        SetupProjectileObj(path, pos);
    }

    protected virtual void SetupProjectileObj(GameObject path, Vector3 pos)
    {
        GameObject obj = Object.Instantiate(path, pos, Quaternion.identity);
        obj.SetActive(false);
        obj.transform.parent = Combat.instanceTransfrom;
        prefab = obj.GetComponent<Projectile>();
        prefab.InitProjectile(this);
    } 

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public float myDamageType 
    { 
        get 
        { 
            if (prefab.isMagic == true) return Caster.MyRanged_M_HitRate + Caster.MagicalProwess - Caster.MagicalIneptitiude; 
            else return Caster.MyRanged_P_HitRate + Caster.MartialProwess - Caster.MartialIneptitiude; 
        } 
        set { } 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void CombatUpdate()
    {
        bool hasMana = prefab.moveProjectile();

        if (hasMana == false)
        {
            prefab.gameObject.SetActive(false);
            isActive = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana)
    {
        throw new System.NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana, Vector3 targetPos)
    {
        isActive = true;
        target = targetPos;
        prefab.gameObject.SetActive(true);
        prefab.Fire(mana, myCaster.transform.position, target);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue()
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue(Vector3 triggerPos)
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue(DroneUnitBody triggeredDrone)
    {
        if (triggeredDrone != Caster)
        {
            return true;
        }
        return false;
    }

    public bool FinalEffectReturnValue(GameObject triggeredObject)
    {
        throw new System.NotImplementedException();
    }

    public void MyRespondAction(ICombatObject obj, Vector3 targetPos, DroneUnitBody otherCaster = null, GameObject triggeredObject = null)
    {
        throw new System.NotImplementedException();
    }
}
