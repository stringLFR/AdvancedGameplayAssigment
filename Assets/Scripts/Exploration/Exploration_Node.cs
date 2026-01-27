using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [SerializeField]
    protected NodeCanvas canvas;

    public NodeCanvas Canvas => canvas;

    [SerializeField]
    protected DecalProjector projector;

    public float intereactDistance {  get; protected set; }

    public abstract void NodeInteract(Exploration_Caravan caravan);

    public abstract void NodeInteract();

    protected float AreaSetUp()
    {
        float size = Random.Range(nodeInnerRadius, nodeOuterRadius);

        projector.size *= size;

        intereactDistance = (1 * size) + 1;

        return size;
    }
}
