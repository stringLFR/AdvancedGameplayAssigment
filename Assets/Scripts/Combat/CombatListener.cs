using System.Collections.Generic;
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

    public static void Init()
    {
        if (instance == null) instance = new CombatListener();
        if (currentCasterChain == null) currentCasterChain = new List<casterData>();
    }

    public static void CleanUp()
    {
        if (instance != null) instance = null;
        if (currentCasterChain != null) currentCasterChain = null;
    }

    public static void Tick(Combat listenTarget)
    {
        //Update listen Data!!!
    }


}
