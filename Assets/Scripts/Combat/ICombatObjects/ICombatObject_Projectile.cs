using System.Runtime.CompilerServices;
using UnityEngine;

public class ICombatObject_Projectile : ICombatObject
{
    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected Projectile prefab;

    protected Vector3 target;

    public ICombatObject_Projectile(GameObject path, Vector3 pos)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void CombatUpdate()
    {
        bool hasMana = prefab.moveProjectile();

        if (hasMana == false)
        {
            prefab.gameObject.SetActive(false);
            isActive = false;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Reactivate(float mana)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Reactivate(float mana, Vector3 targetPos)
    {
        isActive = true;
        target = targetPos;
        prefab.gameObject.SetActive(true);
        prefab.Fire(myCaster.MyMana, myCaster.transform.position, target);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Reactivate(float mana, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }
}
