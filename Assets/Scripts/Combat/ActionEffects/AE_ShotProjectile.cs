using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_ShotProjectile : ActionEffectBase
{

    protected List<ICombatObject_Projectile> projectiles;

    protected AsyncOperationHandle<GameObject> projectile;

    public void SetAssetPath(string path)
    {
        projectile = Addressables.LoadAssetAsync<GameObject>(path);
        projectile.WaitForCompletion();
    }

    protected void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            p.OnSpawn(caster, this);

            p.Reactivate(caster.MyMana, targetPos);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectile.Result);
        projectiles.Add(pNew);

        pNew.OnSpawn(caster, this);
        pNew.Reactivate(caster.MyMana, targetPos);
    }

    public override void TriggerActionEffect(DroneUnitBody caster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, Vector3 targetPos)
    {
        SetUpProjectile(caster, targetPos);
    }

    public override void TriggerActionEffect(DroneUnitBody caster, Vector3[] targetPositions)
    {
        for (int i = 0; i < targetPositions.Length; i++)
        {
            SetUpProjectile(caster, targetPositions[i]);
        }
    }

    public override void TriggerActionEffect(DroneUnitBody caster, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, DroneUnitBody[] otherCasters)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, GameObject[] targetObjs)
    {
        throw new System.NotImplementedException();
    }
}
