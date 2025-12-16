using UnityEngine;

public class ICombatObject_Projectile : ICombatObject
{
    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;


    protected Projectile prefab;

    public ICombatObject_Projectile(GameObject path)
    {
        GameObject obj = Object.Instantiate(path);
        obj.SetActive(false);
        prefab = obj.GetComponent<Projectile>();    
    }

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public void CombatUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
    }

    public void Reactivate(float mana)
    {
        throw new System.NotImplementedException();
    }

    public void Reactivate(float mana, Vector3 targetPos)
    {
        isActive = true;
    }

    public void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }

    public void Reactivate(float mana, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }
}
