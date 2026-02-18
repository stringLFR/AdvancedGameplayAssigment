using ActionFlowStack;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum SupplyType
{
    FOOD, MANA_STORAGE, INTEL, MEDICINE, METALLICS,
}

[Serializable]
public struct SupplyData
{
    public SupplyType Type;

    public int MaxAmount;

    public int currentAmount;

    public int carryAmount;//How hard is it to extract/add supply to/from a node.

    public SupplyData(SupplyType t, int max, int current, int carry)
    {
        Type = t;
        MaxAmount = max;
        currentAmount = current;
        carryAmount = carry;
    }
}

public abstract class Exploration_Node : MonoBehaviour
{
    [SerializeField]
    protected float nodeOuterRadius, nodeInnerRadius;

    [SerializeField]
    protected bool isTaking = true;

    public bool IsTaking => isTaking;

    [SerializeField]
    protected SupplyData[] supplies;

    public SupplyData[] Supplies => supplies;

    [SerializeField]
    protected NodeCanvas canvas;

    public NodeCanvas Canvas => canvas;

    [SerializeField]
    protected DecalProjector projector;

    protected Exploration_Hostile occupier;

    public Exploration_Hostile GetOccupier => occupier;

    public bool assignedCaravan { get; private set; }

    public float intereactDistance {  get; protected set; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void NodeInteract(Exploration_Caravan caravan, SupplyData data)
    {
        canvas.UpdateSlider(data);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void AddOccupier(Exploration_Hostile h)
    {
        occupier = h;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void RemoveOccupier()
    {
        occupier = null;
    }

    public abstract void OnFeedOrEmpty(bool isTaking, Exploration_Caravan c, Exploration e);

    public virtual void NodeInteract()
    {
        if (assignedCaravan == true) return;

        FlowAction_Exploration explorationFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Exploration;

        if (explorationFlowAction == null) return;

        explorationFlowAction.SpawnCaravan(this);

        assignedCaravan = true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void RemoveCaravan()
    {
        assignedCaravan = false;
    }

    protected float AreaSetUp()
    {
        float size = UnityEngine.Random.Range(nodeInnerRadius, nodeOuterRadius);

        projector.size *= size;

        intereactDistance = (size / 2) + 1f;

        assignedCaravan = false;

        canvas.Button.onClick.AddListener(() => NodeInteract());

        foreach(SupplyData d in supplies)
        {
            canvas.SetUpSliders(d);
            canvas.UpdateSlider(d);
        }

        return size;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void CleanUpNode()
    {
        canvas.Button.onClick.RemoveAllListeners();
    }
}
