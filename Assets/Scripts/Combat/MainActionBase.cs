using UnityEngine;
using actions;
using System;
using UnityEngine.Events;

public enum MainActionTypes//This is used to chose what child class of MainActionBase to create!
{
    NONE, RunToPoint,
}

[Serializable]
public struct MainActionStats
{
    public string MainActionName;
    public int manaCost;
    public effectType Effect;
    public MainActionTypes mainActionType;
    public ActionType actionType;
}

public abstract class MainActionBase
{
    public abstract string MainActionName { get; }
    public abstract ActionType ActionType { get; }
    public abstract effectType EffectType { get; }
    public abstract int ManaCost { get; }

    public abstract void Activate(ControllerBase controller, DroneUnitBody user);

    protected void ActivateADS(MainActionBase trigger)
    {
        CombatListener.currentMainAction = trigger;

        FlowAction_Combat.adsInstance.CreateAncestryStack();

        FlowAction_Combat.adsInstance.ActivateAncestryChain();

        CombatListener.currentMainAction = null;
    }
}
