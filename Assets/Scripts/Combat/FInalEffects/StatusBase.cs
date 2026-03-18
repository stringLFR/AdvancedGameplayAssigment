using UnityEngine;

public interface StatusController
{
    public void PrepareStatus(StatusBase status);
}

public abstract class StatusBase
{
    protected ICombatObject controller;
    protected StatusController statusController; //Should be same object as controller!
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

    public virtual void InitStatus(ICombatObject c, StatusController s)
    {
        controller = c;
        statusController = s;
    }

    public virtual void AttachStatus(float mana, DroneUnitBody target)
    {
        targetHost = target;
        startingMana = mana;
        progress = 0f;
        statusController.PrepareStatus(SetupStatus());
    }

    public virtual bool TriggerStatus()
    {

        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * progressSpeed;

        bool isActive = StatusEffect(targetHost);

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Status ran out of mana!");
            return false;
        }

        if (isActive == false) return false;

        return true;
    }

    protected abstract bool StatusEffect(DroneUnitBody target);

    protected abstract StatusBase SetupStatus();
}
