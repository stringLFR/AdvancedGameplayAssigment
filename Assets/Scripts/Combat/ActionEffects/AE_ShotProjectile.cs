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

    protected AsyncOperationHandle<GameObject> projectilePrefab;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void SetAssetPath(string path)
    {
        projectilePrefab = Addressables.LoadAssetAsync<GameObject>(path);
        projectilePrefab.WaitForCompletion();

    }

    protected virtual void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            p.OnSpawn(caster, this,ICombatDelegateTriggers.NONE);

            p.Reactivate(mana, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        pNew.Reactivate(mana, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Shoots one projectile!");

        SetUpProjectile(caster, targetPos, mana);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Shoots {targetPositions.Length} projectiles!");

        for (int i = 0; i < targetPositions.Length; i++)
        {
            SetUpProjectile(caster, targetPositions[i], mana);
        }
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody[] otherCasters)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject[] targetObjs)
    {
        throw new System.NotImplementedException();
    }
}
