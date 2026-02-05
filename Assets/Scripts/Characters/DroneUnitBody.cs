using actions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public bool IsEnemy {  get; private set; }

    public void SetEnemyBool(bool bollean)
    {
        IsEnemy = bollean;
    }

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

        HP = maxHP;
        mana = maxMana;
        sanity = maxSanity;

        myUI.InitUIPanel(maxHP, maxMana, maxSanity,unit.DroneName);
        myUI.SetHealthSlider(maxHP);
        myUI.SetManaSlider(maxMana);
        myUI.SetSanitySlider(maxSanity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void TakeDamage(int hitRate, float manaCost)
    {
        int rand = Random.Range(hitRate - toughness, hitRate);
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
}
