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
            return;
        }

        SetupStatus();

        
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

        return true;
    }

    protected abstract bool StatusEffect();

    protected abstract void SetupStatus();

    protected abstract string dictoKey();
}

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
        return isStunned;
    }
}

public class Status_Knockback : StatusBase
{
    public static string KnockbackKey = "Knockback";

    private Vector3 kockbackDirection = Vector3.zero;

    public Vector3 KnockbackDirection => kockbackDirection;

    private const float minimumKnockBackSpeed = 5f;

    private const float Speed = 5f;

    private const float friction = 0.99f;

    private const float addModifier = 2f;

    protected override string dictoKey()
    {
        return KnockbackKey;
    }

    protected override void SetupStatus()
    {
        manaDrainPerSec = 0;
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
        Host.ProcedualCore.Agent.Move(kockbackDirection * Time.deltaTime * Speed);

        kockbackDirection *= Time.deltaTime * friction;

        if (kockbackDirection.magnitude < minimumKnockBackSpeed)
        {
            return false;
        }

        return true;
    }
}
