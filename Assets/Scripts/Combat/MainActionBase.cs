using UnityEngine;
using actions;
using System;
using UnityEngine.Events;



[Serializable]
public struct MainActionStats
{
    public string MainActionName;
    public string MainActionDescription;
    public int manaCost;
    public effectType Effect;
    public MainActionTypes mainActionType;
    public ActionType actionType;
    public string assetPath;
}

public abstract class MainActionBase
{
    public abstract string MainActionName { get; }
    public abstract ActionType ActionType { get; }
    public abstract effectType EffectType { get; }
    public abstract int ManaCost { get; }

    public abstract string MainActionInfo { get; }

    public abstract void Activate(ControllerBase controller, DroneUnitBody user);

    protected void ActivateADS(MainActionBase trigger)
    {
        CombatListener.currentMainAction = trigger;

        CombatListener.AddLineToCombatText("Triggering Reactions!");

        FlowAction_Combat.adsInstance.CreateAncestryStack();

        FlowAction_Combat.adsInstance.ActivateAncestryChain();

        CombatListener.currentMainAction = null;
    }
}
