using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_SwingMelee : ActionEffectBase
{

    protected List<ICombaatObject_Melee> slashes = new List<ICombaatObject_Melee>();

    protected AsyncOperationHandle<GameObject> meleePrefab;

    public override void SetAssetPath(string path)
    {
        meleePrefab = Addressables.LoadAssetAsync<GameObject>(path);
        meleePrefab.WaitForCompletion();
    }

    protected virtual void SetUpMelee(DroneUnitBody caster, float mana)
    {
        ICombaatObject_Melee m = slashes.Find(p => p.IsActive == false);

        if (m != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.NONE);

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

    public override void DelegateHandler(ICombatObject Iobj)
    {
        throw new System.NotImplementedException();
    }
}

#region status on melee hit!
public class AE_KnockBackCombo : AE_SwingMelee
{
    protected List<ICombatObject_Status> knockBacks = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Status m = knockBacks.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Kncokback);
        k.OnSpawn(Iobj.Caster,this,ICombatDelegateTriggers.NONE);
        knockBacks.Add(k);
        k.MyRespondAction(Iobj);

        Combat.actionEffectObjects.Add(k);
    }

    protected override void SetUpMelee(DroneUnitBody caster, float mana)
    {
        ICombaatObject_Melee m = slashes.Find(p => p.IsActive == false);

        if (m != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            m.Reactivate(mana / 2);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombaatObject_Melee mNew = new ICombaatObject_Melee(meleePrefab.Result, caster.transform.position);
        slashes.Add(mNew);

        mNew.MyActionDelegate += DelegateHandler;

        mNew.RespondActionMana = mana / 2;
        mNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        mNew.Reactivate(mana / 2);

        Combat.actionEffectObjects.Add(mNew);
    }

    ~AE_KnockBackCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_PercingCombo : AE_SwingMelee
{
    protected List<ICombatObject_Status> Leakings = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
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

    protected override void SetUpMelee(DroneUnitBody caster, float mana)
    {
        ICombaatObject_Melee m = slashes.Find(p => p.IsActive == false);

        if (m != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            m.Reactivate(mana / 2);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombaatObject_Melee mNew = new ICombaatObject_Melee(meleePrefab.Result, caster.transform.position);
        slashes.Add(mNew);

        mNew.MyActionDelegate += DelegateHandler;

        mNew.RespondActionMana = mana / 2;
        mNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        mNew.Reactivate(mana / 2);

        Combat.actionEffectObjects.Add(mNew);
    }

    ~AE_PercingCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_StunningCombo : AE_SwingMelee
{
    protected List<ICombatObject_Status> Stunned = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Status m = Stunned.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Stunned);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        Stunned.Add(k);
        k.MyRespondAction(Iobj);

        Combat.actionEffectObjects.Add(k);
    }

    protected override void SetUpMelee(DroneUnitBody caster, float mana)
    {
        ICombaatObject_Melee m = slashes.Find(p => p.IsActive == false);

        if (m != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            m.Reactivate(mana / 2);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombaatObject_Melee mNew = new ICombaatObject_Melee(meleePrefab.Result, caster.transform.position);
        slashes.Add(mNew);

        mNew.MyActionDelegate += DelegateHandler;

        mNew.RespondActionMana = mana / 2;
        mNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        mNew.Reactivate(mana / 2);

        Combat.actionEffectObjects.Add(mNew);
    }

    ~AE_StunningCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_HackingCombo : AE_SwingMelee
{
    protected List<ICombatObject_Status> Hacked = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
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

    protected override void SetUpMelee(DroneUnitBody caster, float mana)
    {
        ICombaatObject_Melee m = slashes.Find(p => p.IsActive == false);

        if (m != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            m.Reactivate(mana / 2);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombaatObject_Melee mNew = new ICombaatObject_Melee(meleePrefab.Result, caster.transform.position);
        slashes.Add(mNew);

        mNew.MyActionDelegate += DelegateHandler;

        mNew.RespondActionMana = mana / 2;
        mNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        mNew.Reactivate(mana / 2);

        Combat.actionEffectObjects.Add(mNew);
    }

    ~AE_HackingCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion
