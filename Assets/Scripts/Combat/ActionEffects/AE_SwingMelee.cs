using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_SwingMelee : ActionEffectBase
{

    protected List<ICombaatObject_Melee> slashes = new List<ICombaatObject_Melee>();

    protected AsyncOperationHandle<GameObject> meleePrefab;

    public virtual void SetAssetPath(string path)
    {
        meleePrefab = Addressables.LoadAssetAsync<GameObject>(path);
        meleePrefab.WaitForCompletion();
    }

    protected virtual void SetUpMelee(DroneUnitBody caster, float mana)
    {
        ICombaatObject_Melee m = slashes.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

            m.Reactivate(mana);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombaatObject_Melee mNew = new ICombaatObject_Melee(meleePrefab.Result, caster.transform.position);
        slashes.Add(mNew);

        mNew.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        mNew.Reactivate(mana);

        Combat.actionEffectObjects.Add(mNew);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Makes one Slash!");

        if (CombatListener.travelers.TryGetValue(caster, out DroneUnitBody body) == true)
        {
            CombatListener.travelers.Remove(body);
        }

        caster.ProcedualCore.Agent.Move((targetPos - caster.transform.position).normalized);

        caster.ProcedualCore.ManualNavRotTarget = targetPos;

        SetUpMelee(caster, mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
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
