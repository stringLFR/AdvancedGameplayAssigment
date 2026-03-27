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

public class AE_NegationZone : AE_CreateArea
{
    protected List<ICombatObject_Status> negated = new List<ICombatObject_Status>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        if (Iobj.RespondActionTarget.AppliedStatusDict.TryGetValue(Status_Negation.NegationKey, out StatusBase status) == true) return;

        ICombatObject_Status n = negated.Find(p => p.IsActive == false);

        if (n != null)
        {
            n.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(n);

            return;
        }

        ICombatObject_Status k = new ICombatObject_Status();

        k.SetupStatus(StatusEnum.Negation);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        negated.Add(k);
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

    ~AE_NegationZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_StunningZone : AE_CreateArea
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

    ~AE_StunningZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Debuff on hit

public class AE_ArmorDownZone : AE_CreateArea
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

    ~AE_ArmorDownZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaShieldDownZone : AE_CreateArea
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

    ~AE_ManaShieldDownZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_CritInflictZone : AE_CreateArea
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

    ~AE_CritInflictZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_StatusInflictZone : AE_CreateArea
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

    ~AE_StatusInflictZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_MartialDownZone : AE_CreateArea
{
    protected List<ICombatObject_Debuff> MartialIneptitiudes = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = MartialIneptitiudes.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.MartialIneptitiude);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        MartialIneptitiudes.Add(k);
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

    ~AE_MartialDownZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_MagicalDownZone : AE_CreateArea
{
    protected List<ICombatObject_Debuff> MagicalIneptitiudes = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = MagicalIneptitiudes.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.MagicalIneptitiude);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        MagicalIneptitiudes.Add(k);
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

    ~AE_MagicalDownZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaDownZone : AE_CreateArea
{
    protected List<ICombatObject_Debuff> ManaDrained = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = ManaDrained.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.ManaDrain);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        ManaDrained.Add(k);
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

    ~AE_ManaDownZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_DecayZone : AE_CreateArea
{
    protected List<ICombatObject_Debuff> HealthDrained = new List<ICombatObject_Debuff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Debuff m = HealthDrained.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Debuff k = new ICombatObject_Debuff();

        k.SetupDebuff(DebuffsEnum.HealthDrain);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        HealthDrained.Add(k);
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

    ~AE_DecayZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion

#region Buff on hit

public class AE_MartialUpZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> MartialProwessed = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = MartialProwessed.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.MartialProwess);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        MartialProwessed.Add(k);
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

    ~AE_MartialUpZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_MagicalUpZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> MagicalProwessed = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = MagicalProwessed.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.MagicalProwess);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        MagicalProwessed.Add(k);
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

    ~AE_MagicalUpZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ArmorUpZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> ArmorPolished = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = ArmorPolished.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.ArmorPolish);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        ArmorPolished.Add(k);
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

    ~AE_ArmorUpZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaShieldUpZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> ManaReinforced = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = ManaReinforced.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.ManaReinforcement);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        ManaReinforced.Add(k);
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

    ~AE_ManaShieldUpZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_CriticalProtectionZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> CriticalProtected = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = CriticalProtected.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.CriticalProtection);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        CriticalProtected.Add(k);
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

    ~AE_CriticalProtectionZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_StatusProtectionZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> StatusProtected = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = StatusProtected.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.StatusProtection);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        StatusProtected.Add(k);
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

    ~AE_StatusProtectionZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_ManaUpZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> ManaRegenerated = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = ManaRegenerated.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.ManaRegeneration);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        ManaRegenerated.Add(k);
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

    ~AE_ManaUpZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

public class AE_HealthUpZone : AE_CreateArea
{
    protected List<ICombatObject_Buff> HealthRegenerated = new List<ICombatObject_Buff>();

    public override void DelegateHandler(ICombatObject Iobj)
    {
        ICombatObject_Buff m = HealthRegenerated.Find(p => p.IsActive == false);

        if (m != null)
        {
            m.MyRespondAction(Iobj);

            Combat.actionEffectObjects.Add(m);

            return;
        }

        ICombatObject_Buff k = new ICombatObject_Buff();

        k.SetupBuff(BuffsEnum.HealthRegeneration);
        k.OnSpawn(Iobj.Caster, this, ICombatDelegateTriggers.NONE);
        HealthRegenerated.Add(k);
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

    ~AE_HealthUpZone()
    {
        for (int i = 0; i < Areas.Count - 1; i++)
        {
            Areas[i].MyActionDelegate -= DelegateHandler;
        }
    }
}

#endregion
