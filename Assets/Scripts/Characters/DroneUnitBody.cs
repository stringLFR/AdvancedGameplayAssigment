using actions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public sealed class DroneUnitBody : MonoBehaviour
{
    [SerializeField]
    private ProcedualCore procedualCore;
    [SerializeField]
    private DroneUIPanel myUI;

    private DroneUnit droneUnit;
    private bool hasInit = false;

    private int maxHP, maxMana, maxSanity, toughness, ranged_P_HitRate, ranged_M_HitRate, melee_P_HitRate, melee_M_HitRate, speed;
    private int HP, mana, sanity;

    private ControllerBase controller;

    private List<ActionNodeBase> reactions;
    private List<MainActionBase> mainActions;

    public List<ActionNodeBase> Reactions => reactions;
    public List<MainActionBase> MainActions => mainActions;
    public ControllerBase Controller => controller;
    public ProcedualCore ProcedualCore => procedualCore;

    #region Buffs

    public int Overdrive = 0;

    #endregion

    #region Debuffs

    #endregion

    public Dictionary<string,StatusBase> AppliedStatusDict = new Dictionary<string,StatusBase>();

    public bool IsEnemy {  get; private set; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetEnemyBool(bool bollean)
    {
        IsEnemy = bollean;
    }


    public int MyMaxHP => maxHP;
    public int MyHP => HP;
    public int MyMana => mana;
    public int MySanity => sanity;
    public int MyToughness => toughness;
    public int MyRanged_P_HitRate => ranged_P_HitRate;
    public int MyRanged_M_HitRate => ranged_M_HitRate;
    public int MyMelee_P_HitRate => melee_P_HitRate;
    public int MyMelee_M_HitRate => melee_M_HitRate;
    public int MySpeed => speed;

    public DroneUnit DroneUnit => droneUnit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DroneUnit = new DroneUnit(); //DEBUG!
        //DroneUnit.TrainDroneUnit(10f,DroneUnit.CoreStatType.STR, DroneUnit.CoreStatType.DEX, DroneUnit.CoreStatType.CHA);
        
    }

    public void Init(DroneUnit unit, ControllerBase controllerBase)
    {
        if (hasInit == true) return;
        hasInit = true;
        mainActions = new List<MainActionBase>();
        reactions = new List<ActionNodeBase>();
        controller = controllerBase;
        droneUnit = unit;
        float lm = droneUnit.GetLevelModifier;
        maxHP = (int)(droneUnit.GetCON * lm + droneUnit.GetWIS * lm) / 2;
        maxMana = (int)(droneUnit.GetCON * lm + droneUnit.GetINT * lm + droneUnit.GetCHA * lm) / 3;
        maxSanity = (int)(droneUnit.GetWIS * lm);
        speed = (int)(droneUnit.GetCON * lm + droneUnit.GetDEX * lm) / 2;
        toughness = (int)(droneUnit.GetCON * lm + droneUnit.GetSTR * lm) / 2;
        ranged_P_HitRate = (int)(droneUnit.GetDEX * lm + droneUnit.GetWIS * lm) / 2;
        ranged_M_HitRate = (int)(droneUnit.GetDEX * lm + droneUnit.GetINT * lm) / 2;
        melee_P_HitRate = (int)(droneUnit.GetSTR * lm + droneUnit.GetDEX * lm) / 2;
        melee_M_HitRate = (int)(droneUnit.GetSTR * lm + droneUnit.GetINT * lm) / 2;

        mana = maxMana;

        sanity = maxSanity - droneUnit.afterCombatStats.SanityDamageTakenPercentile;

        HP = maxHP - droneUnit.afterCombatStats.HPdamageTakenPercentile;

        myUI.gameObject.SetActive(true);
        myUI.InitUIPanel(this);
        myUI.SetHealthSlider(HP);
        myUI.SetManaSlider(maxMana);
        myUI.SetSanitySlider(sanity); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void TakeDamage(int hitRate, float manaCost)
    {
        int rand = UnityEngine.Random.Range(hitRate - toughness, hitRate);
        //rand -+ others 
        int defaultValue = (rand * (int)Mathf.Clamp(manaCost, 1,float.MaxValue)) / toughness;
        //defaultvalue -+ others 

        if (defaultValue <= 0)
        {
            CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} took no damage!");
            return;
        }

        HP -= defaultValue;

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} was dealt {defaultValue} Damage!");

        myUI.SetHealthSlider(HP);

        if (HP <= 0)
        {
            CombatListener.CombatantDied(this);
        }

        
    }

    public void DirectDamage(int damage)
    {
        HP -= damage;

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} was dealt {damage} Direct Damage!");

        myUI.SetHealthSlider(HP);

        if (HP <= 0)
        {
            CombatListener.CombatantDied(this);
        }
    }

    public void SanityDamage(int damage)
    {
        sanity -= damage;

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} was dealt {damage} sanity Damage!");

        myUI.SetSanitySlider(sanity);

        /*
        if (HP <= 0)
        {
            CombatListener.CombatantDied(this);
        }*/
    }

    public void Heal(int amount, float manaCost)
    {
        HP = math.clamp(HP + (int)(amount * manaCost), 0, maxHP);

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} regains {amount} HP!");

        myUI.SetHealthSlider(HP);
    }

    public void ManaSpent(int amount)
    {
        mana = math.clamp(mana - amount, 0, maxMana);

        myUI.SetManaSlider(mana);
    }

    public void RegainMana()//INT is the amount!
    {
        mana = math.clamp(mana + (int)droneUnit.GetINT, 0, maxMana);

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} regains {(int)droneUnit.GetINT} Mana!");

        myUI.SetManaSlider(mana);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            if (AppliedStatusDict.TryGetValue(Status_Knockback.KnockbackKey, out StatusBase status) == true)
            {
                Status_Knockback k = status as Status_Knockback;

                Vector3 bounce = Vector3.Reflect(k.KnockbackDirection, collision.contacts[0].normal);

                k.AddAdditionalKnockBackSpeed(bounce);

                k.reduceKnockBackSpeed(0.5f);

                DirectDamage((int)k.KnockbackDirection.magnitude);//May change it to normal damage method later!

                hit.DirectDamage((int)k.KnockbackDirection.magnitude);
            }
        }
    }
}
