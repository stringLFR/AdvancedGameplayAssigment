using ActionFlowStack;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Node_Hazard : Exploration_Node
{
    [SerializeField]
    protected NavMeshObstacle NavMeshObstacle;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void NodeInteract(Exploration_Caravan caravan, SupplyData d)
    {
        base.NodeInteract(caravan, d);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void NodeInteract()
    {
        base.NodeInteract();
    }

    public override void OnFeedOrEmpty(bool isTaking, Exploration_Caravan c, Exploration e)
    {
        if (isTaking == true)
        {
            c.node.RemoveCaravan();
            e.DefeatedCaravans.Add(c);
            c.hunters.Clear();

            FlowAction_Exploration explorationFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Exploration;

            if (explorationFlowAction != null)
            {
                explorationFlowAction.RemoveNode(this);
                Destroy(this.gameObject);
            }

            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float size = base.AreaSetUp();

        NavMeshObstacle.radius *= size;
        NavMeshObstacle.height *= size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
