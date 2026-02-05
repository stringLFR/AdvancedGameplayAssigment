using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class CombatListener
{
    public enum listenType
    {

    }

    // THis is used to both show how the current reaction chain is structured!
    public struct casterData
    {
        public ActionEffectBase effect;
        public DroneUnitBody caster;
    }

    public static CombatListener instance = null;

    public static List<casterData> currentCasterChain = null;

    public static MainActionBase currentMainAction = null;

    private static Combat Combat = null;

    private static List<string> pastLines = new List<string>();

    public static void Init(Combat c)
    {
        if (instance == null) instance = new CombatListener();
        if (currentCasterChain == null) currentCasterChain = new List<casterData>();
        Combat = c;
    }

    public static DroneUnitBody GetClosesTarget(bool isEnemy, Vector3 pos)
    {
        List<DroneUnitBody> team = isEnemy == false ? Combat.EnemyTeam : Combat.Playerteam;

        float bestDist = float.MaxValue;
        DroneUnitBody best = null;

        foreach (DroneUnitBody target in team)
        {
            float dist = Vector3.Distance(target.transform.position, pos);

            if (dist < bestDist)
            {
                best = target;
                bestDist = dist;
            }
        }

        return best;
    }

    public static void CleanUp()
    {
        if (instance != null) instance = null;
        if (currentCasterChain != null) currentCasterChain = null;
        Combat = null;
    }

    public static void AddLineToCombatText(string info)
    {
        if (Combat == null) return;

        Combat.CombatText.text = "";

        pastLines.Add(info);

        int linesToRemove = pastLines.Count - Combat.CombatTextLineCountMax;

        for (int i = 0; i < linesToRemove; i++)
        {
            pastLines.RemoveAt(0);
        }

        foreach (string line in pastLines)
        {
            Combat.CombatText.text += $"<br>{line}";
        }
    }

    public static void Tick(Combat listenTarget)
    {
        //Update listen Data!!!
    }
}
