using actions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    public int Overdrive = 0; //Adds more main action uses, can cause sanity lost!
    public int MartialProwess = 0; //Increases physical damages by value!
    public int MagicalProwess = 0; //Increases magical damages by value!
    public int ArmorPolish = 0; //increases toughness value on base hitrate damage calc!
    public int ManaReinforcement = 0; //increases toughness value on mana modifier calc, which takes place after base damage calc!
    public int CriticalProtection = 0; //Reduces chance to take crit damage!
    public int MultiHits = 0; //Adds additional basic damage/heal triggers based on value!
    public int ManaRegeneration = 0; //Adds extra mana regained on turn start!
    public int HealthRegeneration = 0; //Regain value based amount of HP on turn start!
    public int StatusProtection = 0; //Makes status damages taken less, based on value!

    public int GetTargetBuffValue(BuffsEnum target)
    {
        switch (target)
        {
            case BuffsEnum.Overdrive:
                return Overdrive;
            case BuffsEnum.MartialProwess:
                return MartialProwess;
            case BuffsEnum.MagicalProwess:
                return MagicalProwess;
            case BuffsEnum.ArmorPolish:
                return ArmorPolish;
            case BuffsEnum.ManaReinforcement:
                return ManaReinforcement;
            case BuffsEnum.CriticalProtection:
                return CriticalProtection;
            case BuffsEnum.Multihit:
                return MultiHits;
            case BuffsEnum.ManaRegeneration:
                return ManaRegeneration;
            case BuffsEnum.HealthRegeneration:
                return HealthRegeneration;
            case BuffsEnum.StatusProtection:
                return StatusProtection;
        }

        return 0;
    }

    #endregion

    #region Debuffs

    public int ArmorBreak = 0; //Reduces toughness value on base hitrate damage calc!
    public int ManaSusceptibility = 0; //Reduces toughness value on mana modifier calc, which takes place after base damage calc!
    public int MartialIneptitiude = 0; //Reduces physical damages by value!
    public int MagicalIneptitiude = 0; //Reduces magical damages by value!
    public int CriticalVulnerability = 0; //Add chance to make damage after mana calc critical (crit value = current damage * 2 + CriticalExploit)!
    public int CriticalExploit = 0; //Makes critical damages taken stronger (crit value = current damage * 2 + CriticalExploit)!
    public int StatusVulnerability = 0; //Makes status damages taken worse. extra damage based on value!
    public int HealthDrain = 0; //lose value based amount of HP on turn start!
    public int ManaDrain = 0; //Removes mana on turn start!

    public int GetTargetDebuffValue(DebuffsEnum target)
    {
        switch (target)
        {
            case DebuffsEnum.ArmorBreak:
                return ArmorBreak;
            case DebuffsEnum.ManaSusceptibility:
                return ManaSusceptibility;
            case DebuffsEnum.MartialIneptitiude:
                return MartialIneptitiude;
            case DebuffsEnum.MagicalIneptitiude:
                return MagicalIneptitiude;
            case DebuffsEnum.CriticalVulnerability:
                return CriticalVulnerability;
            case DebuffsEnum.CriticalExploit:
                return CriticalExploit;
            case DebuffsEnum.StatusVulnerability:
                return StatusVulnerability;
            case DebuffsEnum.HealthDrain:
                return HealthDrain;
            case DebuffsEnum.ManaDrain:
                return ManaDrain;
        }

        return 0;
    }

    public bool GetTargetStatus(StatusEnum target)
    {
        switch (target)
        {
            case StatusEnum.Stunned:
                return AppliedStatusDict.TryGetValue(Status_Stunned.StunnedKey, out StatusBase Stunned);
            case StatusEnum.Kncokback:
                return AppliedStatusDict.TryGetValue(Status_Knockback.KnockbackKey, out StatusBase Kncokback);
            case StatusEnum.Leaking:
                return AppliedStatusDict.TryGetValue(Status_Leaking.LeakingKey, out StatusBase Leaking);
            case StatusEnum.Negation:
                return AppliedStatusDict.TryGetValue(Status_Negation.NegationKey, out StatusBase Negation);
            case StatusEnum.Hacked:
                return AppliedStatusDict.TryGetValue(Status_Hacked.HackedKey, out StatusBase Hacked);
            case StatusEnum.Manaburn:
                return AppliedStatusDict.TryGetValue(Status_ManaBurn.ManaBurnKey, out StatusBase Manaburn);
        }

        return false;
    }

    #endregion

    public Dictionary<string,StatusBase> AppliedStatusDict = new Dictionary<string,StatusBase>();

    public bool IsEnemy {  get; private set; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetEnemyBool(bool bollean)
    {
        IsEnemy = bollean;
    }


    public int MyMaxHP => maxHP;
    public int MyMaxMana => maxMana;
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
        int rand = UnityEngine.Random.Range(hitRate - math.clamp(toughness - ArmorBreak + ArmorPolish, 0, toughness), hitRate);
        //rand -+ others 
        int defaultValue = (rand * (int)Mathf.Clamp(manaCost, 1,float.MaxValue)) / math.clamp(toughness - ManaSusceptibility + ManaReinforcement, 1, toughness);
        //defaultvalue -+ others 

        if (UnityEngine.Random.Range(0f, CriticalVulnerability) > CriticalProtection)
        {
            defaultValue *= 2 + CriticalExploit;

            CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} Got critically hit! Damage is modified by {2 + CriticalExploit}!");
        }

        if (defaultValue <= 0)
        {
            CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} took no damage!");
            return;
        }

        HP -= defaultValue;

        myUI.SetHealthSlider(HP);

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName} was dealt {defaultValue} Damage!");

        if (AppliedStatusDict.TryGetValue(Status_Leaking.LeakingKey, out StatusBase status) == true)
        {
            Status_Leaking leak = status as Status_Leaking;

            leak.IncreaseLeakSpeed();
        }

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
        mana = math.clamp(mana + (int)droneUnit.GetINT + ManaRegeneration - ManaDrain, 0, maxMana);

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName}'s mana is changed by {(int)droneUnit.GetINT + ManaRegeneration - ManaDrain} from int based {(int)droneUnit.GetINT} + manaRegenerationBuff {ManaRegeneration} - ManaDrain debuff {ManaDrain}!");

        myUI.SetManaSlider(mana);
    }

    public void RegainHP()
    {
        if (HealthRegeneration < 1) return;

        HP = math.clamp(HP + HealthRegeneration - HealthDrain, 0, maxHP);

        CombatListener.AddLineToCombatText($"{DroneUnit.DroneName}'s health is changed by {HealthRegeneration - HealthDrain} from HealthRegeneration buff {HealthRegeneration} - HealthDrain debuff {HealthDrain}!");

        myUI.SetHealthSlider(HP);
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

                DirectDamage(k.KnockBackDamage);

                hit.DirectDamage(k.KnockBackDamage - StatusVulnerability + StatusProtection);
            }
        }
    }
}
