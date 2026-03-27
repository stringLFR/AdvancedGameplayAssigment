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
    public override void SetAssetPath(string path)
    {
        projectilePrefab = Addressables.LoadAssetAsync<GameObject>(path);
        projectilePrefab.WaitForCompletion();

    }

    protected virtual void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //p.OnSpawn(caster, this,ICombatDelegateTriggers.NONE);

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

    public override void DelegateHandler(ICombatObject Iobj)
    {
        throw new NotImplementedException();
    }
}

#region Status on tick hit

public class AE_KnockBackShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_KnockBackShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_PiercingShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_PiercingShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_HackShoot : AE_ShotProjectile
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


    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_HackShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaBurnShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_ManaBurnShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Debuff on hit

public class AE_ArmorBreakShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_ArmorBreakShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaSusceptibilityShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_ManaSusceptibilityShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_CriticalVulnerabilityShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_CriticalVulnerabilityShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_StatusVulnerabilityShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_StatusVulnerabilityShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Needs CriticalVulnerability to be above 0 to trigger delegate!
public class AE_CriticalExploitShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_CriticalExploitShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes Ineptitiude type based on highest defence reduction type between ArmorBreak(physical) and ManaSusceptibility(magic)!
public class AE_IneptitiudeShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_IneptitiudeShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes drain type based on highest defence reduction type between ArmorBreak(health) and ManaSusceptibility(mana)!
public class AE_DrainShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_DrainShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Gain buff on hit

//Changes prowess type based on highest defence reduction type between ArmorBreak(martial) and ManaSusceptibility(magical)!
public class AE_ProwessShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_ProwessShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes regeneration type based on highest drain type between healthDrain(health) and manaDrain(mana)!
public class AE_ReapingShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_ReapingShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes defence type based on highest martial type between MartialProwess(ArmorPolish) and MagicalProwess(ManaReinforcement) that the target has!
public class AE_DefenciveShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_DefenciveShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

//Changes protection type based on highest vulnerability type between StatusVulnerability(StatusProtection) and CriticalVulnerability(CriticalProtection)!
public class AE_AnitVulnerabilityShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_AnitVulnerabilityShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}


#endregion

#region Gain positive status on hit

//Gain the negation status, if target has manaburn! It consumes the manaburn from the target!
public class AE_NegationShoot : AE_ShotProjectile
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

    protected override void SetUpProjectile(DroneUnitBody caster, Vector3 targetPos, float mana)
    {
        ICombatObject_Projectile p = projectiles.Find(p => p.IsActive == false);

        if (p != null)
        {
            //m.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);

            p.Reactivate(mana / 2, targetPos);

            Combat.actionEffectObjects.Add(p);

            return;
        }

        ICombatObject_Projectile pNew = new ICombatObject_Projectile(projectilePrefab.Result, caster.transform.position);
        projectiles.Add(pNew);

        pNew.MyActionDelegate += DelegateHandler;

        pNew.RespondActionMana = mana / 2;
        pNew.OnSpawn(caster, this, ICombatDelegateTriggers.ON_DRONEHIT);
        pNew.Reactivate(mana / 2, targetPos);

        Combat.actionEffectObjects.Add(pNew);
    }

    ~AE_NegationShoot()
    {
        for (int i = 0; i < projectiles.Count - 1; i++)
        {
            projectiles[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion
