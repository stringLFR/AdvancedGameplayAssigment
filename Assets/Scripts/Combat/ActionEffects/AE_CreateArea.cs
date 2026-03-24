using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_CreateArea : ActionEffectBase
{

    protected List<ICombatObject_Area> Areas = new List<ICombatObject_Area>();

    protected AsyncOperationHandle<GameObject> AreaPrefab;

    public virtual void SetAssetPath(string path)
    {
        AreaPrefab = Addressables.LoadAssetAsync<GameObject>(path);
        AreaPrefab.WaitForCompletion();
    }

    protected virtual void SetUpArea(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Area a = Areas.Find(p => p.IsActive == false);

        if (a != null)
        {
            a.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

            a.Reactivate(mana, targetPos);

            Combat.actionEffectObjects.Add(a);

            return;
        }

        ICombatObject_Area aNew = new ICombatObject_Area(AreaPrefab.Result, caster.transform.position);
        Areas.Add(aNew);

        aNew.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);
        aNew.Reactivate(mana, targetPos);

        Combat.actionEffectObjects.Add(aNew);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Creates one Area!");

        SetUpArea(caster, targetPos,mana);
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions)
    {
        CombatListener.AddLineToCombatText($"{caster.DroneUnit.DroneName} Makes {targetPositions.Length} Areas!");

        for (int i = 0; i < targetPositions.Length; i++)
        {
            SetUpArea(caster, targetPositions[i],mana);
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
