using UnityEngine;
using ADSNameSpace;
using System.Runtime.CompilerServices;
using System;
using actions;
using System.Collections.Generic;
using System.Collections;



public struct AfterCombatStats
{
    public int expGained;
    public int HPdamageTakenPercentile;
    public int SanityDamageTakenPercentile;
}

public enum CoreStatType
{
    NONE, STR, DEX, CON, INT, WIS, CHA,
}

public struct UpgradePageStruct
{
    public float currentUpgradeValue;
    public CoreStatType full;
    public CoreStatType half;
    public CoreStatType minus;
}



[Serializable]
public sealed class DroneUnit
{
    [SerializeField]
    private string droneName;

    public string DroneName => droneName;

    public UpgradePageStruct myUpgradePageInfo { get; private set; }   

    [SerializeField]
    private float STR = 4f, DEX = 4f, CON = 4f, INT = 4f, WIS = 4f, CHA = 4f;
    [SerializeField]
    private int level = 1;

    public AfterCombatStats afterCombatStats = new AfterCombatStats();

    private int memory;

    [SerializeField]
    private List<ReactionSO> myReactionNodes;

    public List<ActionNodeStats> MyReactionNodes
    {
        get
        {
            List<ActionNodeStats> actionNodeStats = new List<ActionNodeStats>();

            foreach (var node in myReactionNodes) actionNodeStats.Add(node.reactionStats);

            return actionNodeStats;
        }
    }

    [SerializeField]
    private List<MainActionSO> myMainActions;

    public List<MainActionStats> MyMainActions
    {
        get
        {
            List<MainActionStats> mainActionStats = new List<MainActionStats>();

            foreach (var node in myMainActions) mainActionStats.Add(node.mainActionStats);

            return mainActionStats;
        }
    }

    public int Level => level;
    public int Memory => memory;

    public void LevelUp()
    {
        level++;
    }

    public void RerollUpgradeStats(float maxValue)
    {
        UpgradePageStruct newPage = new UpgradePageStruct();

        newPage.currentUpgradeValue = UnityEngine.Random.Range(1, maxValue);

        newPage.full = (CoreStatType)UnityEngine.Random.Range(1, 7);

        CoreStatType[] arr1 = new CoreStatType[0];
        CoreStatType[] arr2 = new CoreStatType[4];

        switch (newPage.full)
        {
            case CoreStatType.STR:

                arr1 = new CoreStatType[5] {CoreStatType.DEX, CoreStatType.CON, CoreStatType.INT, CoreStatType.WIS, CoreStatType.CHA };

                break;
            case CoreStatType.DEX:

                arr1 = new CoreStatType[5] { CoreStatType.STR, CoreStatType.CON, CoreStatType.INT, CoreStatType.WIS, CoreStatType.CHA };

                break;
            case CoreStatType.CON:

                arr1 = new CoreStatType[5] { CoreStatType.STR, CoreStatType.DEX, CoreStatType.INT, CoreStatType.WIS, CoreStatType.CHA };

                break;
            case CoreStatType.INT:

                arr1 = new CoreStatType[5] { CoreStatType.STR, CoreStatType.DEX, CoreStatType.CON, CoreStatType.WIS, CoreStatType.CHA };

                break;
            case CoreStatType.WIS:

                arr1 = new CoreStatType[5] { CoreStatType.STR, CoreStatType.DEX, CoreStatType.CON, CoreStatType.INT, CoreStatType.CHA };

                break;
            case CoreStatType.CHA:

                arr1 = new CoreStatType[5] { CoreStatType.STR, CoreStatType.DEX, CoreStatType.CON, CoreStatType.INT, CoreStatType.WIS };

                break;
        }

        newPage.half = arr1[UnityEngine.Random.Range(0, arr1.Length - 1)];

        int index = 0;

        switch (newPage.half)
        {
            case CoreStatType.STR:

                for(int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] == CoreStatType.STR) continue;

                    arr2[index] = arr1[i];

                    index++;
                }

                break;
            case CoreStatType.DEX:

                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] == CoreStatType.DEX) continue;

                    arr2[index] = arr1[i];

                    index++;
                }

                break;
            case CoreStatType.CON:

                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] == CoreStatType.CON) continue;

                    arr2[index] = arr1[i];

                    index++;
                }

                break;
            case CoreStatType.INT:

                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] == CoreStatType.INT) continue;

                    arr2[index] = arr1[i];

                    index++;
                }

                break;
            case CoreStatType.WIS:

                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] == CoreStatType.WIS) continue;

                    arr2[index] = arr1[i];

                    index++;
                }

                break;
            case CoreStatType.CHA:

                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1[i] == CoreStatType.CHA) continue;

                    arr2[index] = arr1[i];

                    index++;
                }
                break;
        }

        newPage.minus = arr2[UnityEngine.Random.Range(0, arr2.Length - 1)];

        myUpgradePageInfo = newPage;
    }

    public int CombatRating
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get
        {
            float lm = GetLevelModifier;
            return (int)(STR * lm + DEX * lm + CON * lm + INT * lm + WIS * lm + CHA * lm) / 6;
        }
    }
    public float GetLevelModifier { get { return 1f + level / 10; } }
    public float GetSTR {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return STR; } }
    public float GetDEX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return DEX; } }
    public float GetCON {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return CON; } }
    public float GetINT {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return INT; } }
    public float GetWIS {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return WIS; } }
    public float GetCHA {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return CHA; } }


    // main gets full value, secondary gets half value, while sacrifice is reduced by a 1/4 of the value!
    public void SimpleTrainDroneUnit(float TrainingValue, CoreStatType stat)
    {
        switch (stat)
        {
            case CoreStatType.STR:
                STR += TrainingValue;
                break;
            case CoreStatType.DEX:
                DEX += TrainingValue;
                break;
            case CoreStatType.CON:
                CON += TrainingValue;
                break;
            case CoreStatType.INT:
                INT += TrainingValue;
                break;
            case CoreStatType.WIS:
                WIS += TrainingValue;
                break;
            case CoreStatType.CHA:
                CHA += TrainingValue;
                break;
        }
    }

    public void TrainDroneUnit(float TrainingValue, CoreStatType fullValue, CoreStatType halfValue, CoreStatType oneFourthReduction)
    {
        switch (fullValue)
        {
            case CoreStatType.STR:
                STR += TrainingValue;
                break;
            case CoreStatType.DEX:
                DEX += TrainingValue;
                break;
            case CoreStatType.CON:
                CON += TrainingValue;
                break;
            case CoreStatType.INT:
                INT += TrainingValue;
                break;
            case CoreStatType.WIS:
                WIS += TrainingValue;
                break;
            case CoreStatType.CHA:
                CHA += TrainingValue;
                break;
        }
        switch (halfValue)
        {
            case CoreStatType.STR:
                STR += (TrainingValue / 2);
                break;
            case CoreStatType.DEX:
                DEX += (TrainingValue / 2);
                break;
            case CoreStatType.CON:
                CON += (TrainingValue / 2);
                break;
            case CoreStatType.INT:
                INT += (TrainingValue / 2);
                break;
            case CoreStatType.WIS:
                WIS += (TrainingValue / 2);
                break;
            case CoreStatType.CHA:
                CHA += (TrainingValue / 2);
                break;
        }
        switch (oneFourthReduction)
        {
            case CoreStatType.STR:
                STR -= (TrainingValue / 4);
                break;
            case CoreStatType.DEX:
                DEX -= (TrainingValue / 4);
                break;
            case CoreStatType.CON:
                CON -= (TrainingValue / 4);
                break;
            case CoreStatType.INT:
                INT -= (TrainingValue / 4);
                break;
            case CoreStatType.WIS:
                WIS -= (TrainingValue / 4);
                break;
            case CoreStatType.CHA:
                CHA -= (TrainingValue / 4);
                break;
        }
    }
}