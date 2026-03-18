using UnityEngine;

public interface BuffORDebuffController
{
    public void PrepareBuffORDebuff(BuffORDebuffBase buffDebuff);
}

public abstract class BuffORDebuffBase
{
    protected ICombatObject controller;
    protected BuffORDebuffController buffDebuffController; //Should be same object as controller!
    protected float startingMana;
    protected float progress;
    protected float progressSpeed = 1f;
    protected DroneUnitBody targetHost;
    protected float baseDamage = 1f, manaDrainPerSec = 1f;

    public DroneUnitBody Host => targetHost;

    protected bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    protected bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }

    public virtual void InitStatus(ICombatObject c, BuffORDebuffController s)
    {
        controller = c;
        buffDebuffController = s;
    }

    public virtual void AttachStatus(float mana, DroneUnitBody target)
    {
        targetHost = target;
        startingMana = mana;
        progress = 0f;
        buffDebuffController.PrepareBuffORDebuff(SetupBuffDebuff());
    }

    public virtual bool TriggerStatus()
    {

        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * progressSpeed;

        bool isActive = BuffDebuffEffect(targetHost);

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"BuffDebuff ran out of mana!");
            return false;
        }

        if (isActive == false) return false;

        return true;
    }

    protected abstract bool BuffDebuffEffect(DroneUnitBody target);

    protected abstract BuffORDebuffBase SetupBuffDebuff();
}
