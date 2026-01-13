using UnityEngine;

public enum SupplyType
{
    FOOD, MANA_STORAGE, INTEL, MEDICINE, METALLICS,
}

public abstract class Exploration_Node : MonoBehaviour
{
    [SerializeField]
    protected float nodeOuterRadius, nodeInnerRadius;

    [SerializeField]
    protected bool givesResources = true;

    [SerializeField]
    protected SupplyType[] supplies;

    [SerializeField]
    protected int[] supplyAmmounts;
}
