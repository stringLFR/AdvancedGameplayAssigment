using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_ShotProjectile : ActionEffectBase
{

    protected List<ICombatObject_Projectile> projectiles = new List<ICombatObject_Projectile>();

    protected AsyncOperationHandle<GameObject> projectile;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
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

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectile.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.OnSpawn(caster, this);
        pNew.Reactivate(caster.MyMana, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    public override void TriggerActionEffect(DroneUnitBody caster)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(DroneUnitBody caster, Vector3 targetPos)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Shoots one projectile!");

        SetUpProjectile(caster, targetPos);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(DroneUnitBody caster, Vector3[] targetPositions)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Shoots {targetPositions.Length} projectiles!");

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
