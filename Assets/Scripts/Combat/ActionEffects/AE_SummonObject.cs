using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_SummonObject : ActionEffectBase
{
    protected List<ICombatObject_SummonObject> Summons = new List<ICombatObject_SummonObject>();

    protected AsyncOperationHandle<GameObject> SummonPrefab;

    public override void SetAssetPath(string path)
    {
        SummonPrefab = Addressables.LoadAssetAsync<GameObject>(path);
        SummonPrefab.WaitForCompletion();
    }

    protected virtual void SetUpSummon(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_SummonObject s = Summons.Find(p => p.IsActive == false);

        if (s != null)
        {
            //s.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

            s.Reactivate(mana, targetPos);

            Combat.actionEffectObjects.Add(s);

            return;
        }

        ICombatObject_SummonObject sNew = new ICombatObject_SummonObject(SummonPrefab.Result, caster.transform.position);
        Summons.Add(sNew);

        sNew.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        sNew.Reactivate(mana, targetPos);

        Combat.actionEffectObjects.Add(sNew);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Summons one Object!");

        SetUpSummon(caster, targetPos, mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Summons {targetPositions.Length} Objects!");

        for (int i = 0; i < targetPositions.Length; i++)
        {
            SetUpSummon(caster, targetPositions[i], mana);
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

    public override void DelegateHandler(ICombatObject Iobj)
    {
        throw new System.NotImplementedException();
    }
}
