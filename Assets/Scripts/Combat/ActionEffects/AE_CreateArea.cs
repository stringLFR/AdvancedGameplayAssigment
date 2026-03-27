using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_CreateArea : ActionEffectBase
{

    protected List<ICombatObject_Area> Areas = new List<ICombatObject_Area>();

    protected AsyncOperationHandle<GameObject> AreaPrefab;

    public override void SetAssetPath(string path)
    {
        AreaPrefab = Addressables.LoadAssetAsync<GameObject>(path);
        AreaPrefab.WaitForCompletion();
    }

    protected virtual void SetUpArea(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Area a = Areas.Find(p => p.IsActive == false);

        if (a != null)
        {
            //a.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

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

    public override void DelegateHandler(ICombatObject Iobj)
    {
        throw new System.NotImplementedException();
    }
}

#region Status on tick hit

public class AE_KnockBackZone : AE_CreateArea
{
    protected List<ICombatObject_Status> knockBacks = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_Knockback.KnockbackKey, out StatusBase status) == true) return;

        ICombatObject_Status m = knockBacks.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Kncokback);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        knockBacks.Add(k);
        k.MyRespondAction(Iobj);

        Combat.actionEffectObjects.Add(k);
    }

    protected override void SetUpArea(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Area a = Areas.Find(p => p.IsActive == false);

        if (a != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            a.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(a);

            return;
        }

        ICombatObject_Area aNew = new ICombatObject_Area(AreaPrefab.Result, caster.transform.position);
        Areas.Add(aNew);

        aNew.MyActionDelegate += DelegateHandler;

        aNew.RespondActionMana = mana / 2;
        aNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        aNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(aNew);
    }

    ~AE_KnockBackZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ShrapnelZone : AE_CreateArea
{
    protected List<ICombatObject_Status> Leakings = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_Leaking.LeakingKey, out StatusBase status) == true) return;

        ICombatObject_Status m = Leakings.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Leaking);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        Leakings.Add(k);
        k.MyRespondAction(Iobj);

        Combat.actionEffectObjects.Add(k);
    }

    protected override void SetUpArea(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Area a = Areas.Find(p => p.IsActive == false);

        if (a != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            a.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(a);

            return;
        }

        ICombatObject_Area aNew = new ICombatObject_Area(AreaPrefab.Result, caster.transform.position);
        Areas.Add(aNew);

        aNew.MyActionDelegate += DelegateHandler;

        aNew.RespondActionMana = mana / 2;
        aNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        aNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(aNew);
    }

    ~AE_ShrapnelZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_HackingZone : AE_CreateArea
{
    protected List<ICombatObject_Status> Hacked = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_Hacked.HackedKey, out StatusBase status) == true) return;

        ICombatObject_Status m = Hacked.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Hacked);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        Hacked.Add(k);
        k.MyRespondAction(Iobj);

        Combat.actionEffectObjects.Add(k);
    }

    protected override void SetUpArea(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Area a = Areas.Find(p => p.IsActive == false);

        if (a != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            a.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(a);

            return;
        }

        ICombatObject_Area aNew = new ICombatObject_Area(AreaPrefab.Result, caster.transform.position);
        Areas.Add(aNew);

        aNew.MyActionDelegate += DelegateHandler;

        aNew.RespondActionMana = mana / 2;
        aNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        aNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(aNew);
    }

    ~AE_HackingZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaBurnZone : AE_CreateArea
{
    protected List<ICombatObject_Status> burning = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_ManaBurn.ManaBurnKey, out StatusBase status) == true) return;

        ICombatObject_Status m = burning.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Manaburn);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        burning.Add(k);
        k.MyRespondAction(Iobj);

        Combat.actionEffectObjects.Add(k);
    }

    protected override void SetUpArea(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Area a = Areas.Find(p => p.IsActive == false);

        if (a != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            a.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(a);

            return;
        }

        ICombatObject_Area aNew = new ICombatObject_Area(AreaPrefab.Result, caster.transform.position);
        Areas.Add(aNew);

        aNew.MyActionDelegate += DelegateHandler;

        aNew.RespondActionMana = mana / 2;
        aNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        aNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(aNew);
    }

    ~AE_ManaBurnZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion
