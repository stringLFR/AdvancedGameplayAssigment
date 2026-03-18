using UnityEngine;


public abstract class BuffORDebuffBase
{
    protected ICombatObject controller;
    protected float startingMana;
    protected DroneUnitBody targetHost;
    protected float baseDamage = 1f, manaDrainPerSec = 1f;

    public DroneUnitBody Host => targetHost;

    protected bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    protected bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }

    public virtual void InitBuffDebuff(ICombatObject c)
    {
        controller = c;
    }

    public virtual void AttachBuffDebuff(float mana, DroneUnitBody target)
    {
        targetHost = target;
        startingMana = mana;
        SetupBuffDebuff();
    }

    public virtual bool BuffDebuffDuration()
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"BuffDebuff ran out of mana!");

            EndBuffDebuff();

            return false;
        }

        return true;
    }

    protected abstract void EndBuffDebuff();

    protected abstract void SetupBuffDebuff();
}

public class OverdriveBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.Overdrive--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.Overdrive++;

        manaDrainPerSec = 1f;
    }
}
