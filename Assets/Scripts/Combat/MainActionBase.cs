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
    public ActionType[] reactionTypes;
    public string assetPath;
    public AIDecisionType AIDecisionType;
    public int AIDecisionEnumTarget;

    [Range(0,2)]
    public float AIBaseInputModifier;
    [Range(0, 2)]
    public float AIDistanceInputModifier;
    [Range(0, 2)]
    public float AIRandomInputModifier;
    [Range(0, 2)]
    public float AIManaInoutModifier;
    [Range(0, 2)]
    public float AIPriorityInputModifier;
}

public abstract class MainActionBase
{
    public abstract string MainActionName { get; }
    public abstract ActionType ActionType { get; }
    public abstract ActionType[] ReactionTypes { get; }

    public abstract int EnumTarget {  get; }
    public abstract effectType EffectType { get; }

    public abstract AIDecisionType AIDecisionType { get; }
    public abstract int ManaCost { get; }

    public abstract string MainActionInfo { get; }

    public float AIBaseInputModifier;
    public float AIDistanceInputModifier;
    public float AIRandomInputModifier;
    public float AIManaInputModifier;
    public float AIPriorityInputModifier;

    public abstract void Activate(ControllerBase controller, DroneUnitBody user);

    public void SetAIDecisionModifiers(MainActionStats stats)
    {
        AIBaseInputModifier = stats.AIBaseInputModifier;
        AIDistanceInputModifier = stats.AIDistanceInputModifier;
        AIRandomInputModifier = stats.AIRandomInputModifier;
        AIManaInputModifier = stats.AIManaInoutModifier;
        AIPriorityInputModifier = stats.AIPriorityInputModifier;
    }

    protected void ActivateADS(MainActionBase trigger)
    {
        CombatListener.currentMainAction = trigger;

        CombatListener.AddLineToCombatText("Triggering Reactions!");

        FlowAction_Combat.adsInstance.CreateAncestryStack();

        FlowAction_Combat.adsInstance.ActivateAncestryChain();

        CombatListener.currentMainAction = null;
    }
}
