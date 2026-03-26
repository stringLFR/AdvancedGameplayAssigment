using UnityEngine;


public abstract class StatusBase
{
    protected ICombatObject controller;
    protected float startingMana;
    protected float progress;
    protected float progressSpeed = 1f;
    protected DroneUnitBody targetHost;
    protected float baseDamage = 1f, manaDrainPerSec = 1f;

    protected bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    protected bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }

    public DroneUnitBody Host => targetHost;

    private bool didNotAttach = false;

    public virtual void InitStatus(ICombatObject c)
    {
        controller = c;
    }

    public virtual void AttachStatus(float mana, DroneUnitBody target)
    {
        targetHost = target;
        startingMana = mana;
        progress = 0f;
        didNotAttach = false;

        if (targetHost.AppliedStatusDict.TryAdd(dictoKey(), this) == false)
        {
            didNotAttach = true;

            CombatListener.AddLineToCombatText($"Status is already applied!");

            return;
        }

        SetupStatus();
        controller.TriggerDelegate();
        
    }

    public virtual bool TriggerStatus()
    {
        if (didNotAttach == true)
        {
            return false;
        }

        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * progressSpeed;

        bool isActive = StatusEffect();

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Status ran out of mana!");

            targetHost.AppliedStatusDict.Remove(dictoKey());

            return false;
        }

        if (isActive == false)
        {
            targetHost.AppliedStatusDict.Remove(dictoKey());

            return false;
        }

        if (Host.MyHP <= 0) return false;

        return true;
    }

    protected abstract bool StatusEffect();

    protected abstract void SetupStatus();

    protected abstract string dictoKey();
}

public enum StatusEnum
{
    None,Stunned,Kncokback,Leaking,Negation,Hacked,Manaburn,
}

//Stuns, Do not need mana to sustain!
public class Status_Stunned : StatusBase
{
    public static string StunnedKey = "Stunned";

    private bool isStunned = false;

    public void TakeOffStunned()
    {
        isStunned = false;
    }

    protected override string dictoKey()
    {
        return StunnedKey;
    }

    protected override void SetupStatus()
    {
        isStunned = true;
        manaDrainPerSec = 0;
    }

    protected override bool StatusEffect()
    {
        if (Host.AppliedStatusDict.TryGetValue(Status_Negation.NegationKey, out StatusBase status) == true)
        {
            return false;
        }

        return isStunned;
    }
}
//Adds knock back that causes damage on colliding. Needs mana to sustain!
public class Status_Knockback : StatusBase
{
    public static string KnockbackKey = "Knockback";

    private Vector3 kockbackDirection = Vector3.zero;

    public Vector3 KnockbackDirection => kockbackDirection;

    private const float minimumKnockBackSpeed = 5f;

    private const float Speed = 5f;

    private const float friction = 100f;

    private const float addModifier = 10f;

    private const float damageModifier = 1f;

    public int KnockBackDamage => (int)(KnockbackDirection.magnitude * damageModifier) + Host.StatusVulnerability - Host.StatusProtection;

    protected override string dictoKey()
    {
        return KnockbackKey;
    }

    protected override void SetupStatus()
    {
        manaDrainPerSec = 0.5f;
        AddAdditionalKnockBackSpeed((Host.transform.position - controller.Caster.transform.position).normalized);
    }

    public void AddAdditionalKnockBackSpeed(Vector3 addedDir)
    {
        kockbackDirection += addedDir * addModifier;
    }

    public void reduceKnockBackSpeed(float reduction)
    {
        kockbackDirection *= reduction;
    }

    protected override bool StatusEffect()
    {
        if (Host.AppliedStatusDict.TryGetValue(Status_Negation.NegationKey, out StatusBase status) == true)
        {
            return false;
        }

        Host.ProcedualCore.Agent.Move(kockbackDirection * Time.deltaTime * Speed);

        kockbackDirection *= Time.deltaTime * friction;

        if (kockbackDirection.magnitude < minimumKnockBackSpeed)
        {
            return false;
        }

        return true;
    }
}
//Deals low damage on tick, but tick rate increases on being hit. Do not need mana to sustain! (is removed on healing above a damage cap!)
public class Status_Leaking : StatusBase
{
    public static string LeakingKey = "Leaking";

    private const float leakTimer = 1f;

    private const float leakIncreaseValue = 0.1f;

    private float leakProgress = 0f;

    private float leakSpeed;

    private int lastHostHPValue;

    public void IncreaseLeakSpeed()
    {
        leakSpeed += leakIncreaseValue;
    }

    protected override string dictoKey()
    {
        return LeakingKey;
    }

    protected override void SetupStatus()
    {
        manaDrainPerSec = 0;
        leakSpeed = 1f;
        lastHostHPValue = Host.MyHP;
    }

    protected override bool StatusEffect()
    {
        if (Host.AppliedStatusDict.TryGetValue(Status_Negation.NegationKey, out StatusBase status) == true)
        {
            return false;
        }

        if (lastHostHPValue < Host.MyHP) return false;

        leakProgress += Time.deltaTime * leakSpeed;

        if (leakProgress >= leakTimer)
        {
            leakProgress = 0f;

            Host.DirectDamage(1 + Host.StatusVulnerability - Host.StatusProtection);

            lastHostHPValue = Host.MyHP;
        }

        return true;
    }
}
//Both deals damage and reduces mana based on current mana amount/value. Do not need mana to sustain! (It drains the target afterall XD)
public class Status_ManaBurn : StatusBase
{
    public static string ManaBurnKey = "ManaBurn";

    private float manaBurnSplitValue = 4f;

    private float triggerRate = 1f;

    private float triggerProgress = 0f;

    private bool isRemoved = false;

    public void RemoveManaBurn()
    {
        isRemoved = true;
    }

    protected override string dictoKey()
    {
        return ManaBurnKey;
    }

    protected override void SetupStatus()
    {
        manaDrainPerSec = 0;
        isRemoved = false;
    }

    public int ManaBurnDealDamage()
    {
        int burnDamage = (int)(Host.MyMana / manaBurnSplitValue);
        Host.DirectDamage(burnDamage + Host.StatusVulnerability - Host.StatusProtection);
        Host.ManaSpent(burnDamage + Host.StatusVulnerability - Host.StatusProtection);

        return burnDamage;
    }

    protected override bool StatusEffect()
    {
        if (Host.AppliedStatusDict.TryGetValue(Status_Negation.NegationKey, out StatusBase status) == true)
        {
            return false;
        }

        if (isRemoved == true) return false;

        if (Host.MyMana <= 0) return false;

        triggerProgress += Time.deltaTime;

        if (triggerProgress >= triggerRate)
        {
            ManaBurnDealDamage();
        }

        return true;
    }
}
//The main way to protect from statuses. Is active for as long as it has mana! 
public class Status_Negation : StatusBase
{
    public static string NegationKey = "Negation";

    protected override string dictoKey()
    {
        return NegationKey;
    }

    protected override void SetupStatus()
    {
        manaDrainPerSec = 5;
    }

    protected override bool StatusEffect()
    {
        return true;
    }
}
//Reverses the effect of buffs and debuffs when gained. Needs mana to sustain!
//Was planning switching already exisiting ones, but it would take to much refacotring! So doing on gained instead!
public class Status_Hacked : StatusBase
{
    public static string HackedKey = "Hacked";

    protected override string dictoKey()
    {
        return HackedKey;
    }

    protected override void SetupStatus()
    {
        manaDrainPerSec = 5;
    }

    protected override bool StatusEffect()
    {
        return true;
    }
}
