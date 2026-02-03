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

    public static void Tick(Combat listenTarget)
    {
        //Update listen Data!!!
    }
}
