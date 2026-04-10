using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AE_SwingMelee : ActionEffectBase
{

    protected List<ICombaatObject_Melee> slashes = new List<ICombaatObject_Melee>();

    protected AsyncOperationHandle<GameObject> meleePrefab;

    protected const float warpSpeed = 0.625f;

    protected const float warpSpeedMovement = 10f;

    protected IEnumerator WarpStep(Vector3 targetPos, DroneUnitBody caster, float mana)
    {
        NavMeshPath path = new NavMeshPath();

        NavMesh.SamplePosition(targetPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

        NavMesh.CalculatePath(caster.transform.position, hit.position, NavMesh.AllAreas, path);

        int points = path.corners.Length;
        int pointIndex = 0;
        Vector3 directionVelocity = Vector3.zero;

        float t = 0;

        while (true)
        {
            t += Time.deltaTime * warpSpeed;

            if (pointIndex >= points) break;

            if (Vector3.Distance(caster.transform.position, path.corners[pointIndex] + (caster.ProcedualCore.Agent.baseOffset * Vector3.up)) <= caster.ProcedualCore.Agent.radius)
            {
                pointIndex++;
                t = 0;

                if (pointIndex >= points) break;
            }

            Vector3 movementVecotr = (path.corners[pointIndex] + (caster.ProcedualCore.Agent.baseOffset * Vector3.up) - caster.transform.position).normalized;

            directionVelocity = Vector3.Lerp(directionVelocity, movementVecotr, Mathf.Clamp01(t));

            caster.ProcedualCore.Agent.Move(directionVelocity * caster.ProcedualCore.Agent.speed * warpSpeedMovement * Time.deltaTime);

            caster.ProcedualCore.ManualNavRotTarget = path.corners[pointIndex];

            yield return null;
        }

        SetUpMelee(caster, mana);

        yield return null;
    }

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

        caster.StartCoroutine(WarpStep(targetPos,caster,mana));
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
        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_Stunned.StunnedKey, out StatusBase status) == true) return;

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

public class AE_ManaburnCombo : AE_SwingMelee
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

    ~AE_ManaburnCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Debuff on hit

public class AE_ArmorBreakCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> armorBroken = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = armorBroken.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.ArmorBreak);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        armorBroken.Add(k);
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

    ~AE_ArmorBreakCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaSusceptibilityCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> manaSusceptible = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = manaSusceptible.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.ManaSusceptibility);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        manaSusceptible.Add(k);
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

    ~AE_ManaSusceptibilityCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_CriticalVulnerabilityCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> critVulnerables = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = critVulnerables.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.CriticalVulnerability);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        critVulnerables.Add(k);
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

    ~AE_CriticalVulnerabilityCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_StatusVulnerabilityCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> statusVulnerables = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = statusVulnerables.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.StatusVulnerability);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        statusVulnerables.Add(k);
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

    ~AE_StatusVulnerabilityCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Needs CriticalVulnerability to be above 0 to trigger delegate!
public class AE_CriticalExploitCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> critExploited = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (Iobj.RespondActionTarget.CriticalVulnerability <= 0) return;

        ICombatObject_Debuff m = critExploited.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.CriticalExploit);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        critExploited.Add(k);
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

    ~AE_CriticalExploitCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes Ineptitiude type based on highest defence reduction type between ArmorBreak(physical) and ManaSusceptibility(magic)!
public class AE_IneptitiudeCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> ineptituded = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        int magic = Iobj.RespondActionTarget.ManaSusceptibility;
        int physical = Iobj.RespondActionTarget.ArmorBreak;

        if (physical <= 0 && magic <= 0) return;

        ICombatObject_Debuff m = ineptituded.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        if (magic > physical) k.SetupDebuff(DebuffsEnum.MagicalIneptitiude);
        else k.SetupDebuff(DebuffsEnum.MartialIneptitiude);

        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        ineptituded.Add(k);
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

    ~AE_IneptitiudeCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes drain type based on highest defence reduction type between ArmorBreak(health) and ManaSusceptibility(mana)!
public class AE_DrainCombo : AE_SwingMelee
{
    protected List<ICombatObject_Debuff> ineptituded = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        int magic = Iobj.RespondActionTarget.ManaSusceptibility;
        int physical = Iobj.RespondActionTarget.ArmorBreak;

        if (physical <= 0 && magic <= 0) return;

        ICombatObject_Debuff m = ineptituded.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        if (magic > physical) k.SetupDebuff(DebuffsEnum.ManaDrain);
        else k.SetupDebuff(DebuffsEnum.HealthDrain);

        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        ineptituded.Add(k);
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

    ~AE_DrainCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Gain buff on hit

//Changes prowess type based on highest defence reduction type between ArmorBreak(martial) and ManaSusceptibility(magical)!
public class AE_ProwessCombo : AE_SwingMelee
{
    protected List<ICombatObject_Buff> prowessed = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        int magic = Iobj.RespondActionTarget.ManaSusceptibility;
        int physical = Iobj.RespondActionTarget.ArmorBreak;

        if (physical <= 0 && magic <= 0) return;

        ICombatObject_Buff m = prowessed.Find(p => p.IsActive == false);

        if (m != null)
        {
            Iobj.RespondActionTarget = Iobj.Caster;
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        if (magic > physical) k.SetupBuff(BuffsEnum.MagicalProwess);
        else k.SetupBuff(BuffsEnum.MartialProwess);

        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        prowessed.Add(k);
        Iobj.RespondActionTarget = Iobj.Caster;
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

    ~AE_ProwessCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes regeneration type based on highest drain type between healthDrain(health) and manaDrain(mana)!
public class AE_ReapingCombo : AE_SwingMelee
{
    protected List<ICombatObject_Buff> regens = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        int magic = Iobj.RespondActionTarget.ManaDrain;
        int physical = Iobj.RespondActionTarget.HealthDrain;

        if (physical <= 0 && magic <= 0) return;

        ICombatObject_Buff m = regens.Find(p => p.IsActive == false);

        if (m != null)
        {
            Iobj.RespondActionTarget = Iobj.Caster;
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        if (magic > physical) k.SetupBuff(BuffsEnum.ManaRegeneration);
        else k.SetupBuff(BuffsEnum.HealthRegeneration);

        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        regens.Add(k);
        Iobj.RespondActionTarget = Iobj.Caster;
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

    ~AE_ReapingCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes defence type based on highest martial type between MartialProwess(ArmorPolish) and MagicalProwess(ManaReinforcement) that the target has!
public class AE_DefenciveCombo : AE_SwingMelee
{
    protected List<ICombatObject_Buff> defences = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        int magic = Iobj.RespondActionTarget.MagicalProwess;
        int physical = Iobj.RespondActionTarget.MartialProwess;

        if (physical <= 0 && magic <= 0) return;

        ICombatObject_Buff m = defences.Find(p => p.IsActive == false);

        if (m != null)
        {
            Iobj.RespondActionTarget = Iobj.Caster;
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        if (magic > physical) k.SetupBuff(BuffsEnum.ManaReinforcement);
        else k.SetupBuff(BuffsEnum.ArmorPolish);

        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        defences.Add(k);
        Iobj.RespondActionTarget = Iobj.Caster;
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

    ~AE_DefenciveCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes protection type based on highest vulnerability type between StatusVulnerability(StatusProtection) and CriticalVulnerability(CriticalProtection)!
public class AE_AnitVulnerabilityCombo : AE_SwingMelee
{
    protected List<ICombatObject_Buff> defences = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        int status = Iobj.Caster.StatusVulnerability;
        int crit = Iobj.Caster.CriticalVulnerability;

        if (status <= 0 && crit <= 0) return;

        ICombatObject_Buff m = defences.Find(p => p.IsActive == false);

        if (m != null)
        {
            Iobj.RespondActionTarget = Iobj.Caster;
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        if (status > crit) k.SetupBuff(BuffsEnum.StatusProtection);
        else k.SetupBuff(BuffsEnum.CriticalProtection);

        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        defences.Add(k);
        Iobj.RespondActionTarget = Iobj.Caster;
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

    ~AE_AnitVulnerabilityCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Gain positive status on hit

//Gain the negation status, if target has manaburn! It consumes the manaburn from the target!
public class AE_NegationCombo : AE_SwingMelee
{
    protected ICombatObject_Status negate = new ICombatObject_Status();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (negate.IsActive == true) return;

        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_ManaBurn.ManaBurnKey, out StatusBase status) == true)
        {
            Status_ManaBurn burn = status as Status_ManaBurn;
            burn.RemoveManaBurn();

            negate.SetupStatus(StatusEnum.Negation);
            negate.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
            Iobj.RespondActionTarget = Iobj.Caster;
            negate.MyRespondAction(Iobj);
            Combat.actionEffectObjects.Add(negate);
        }
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

    ~AE_NegationCombo()
    {
        for (int i = 0; i < slashes.Count - 1; i++)
        {
            slashes[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion
