using ActionFlowStack;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Exploration_Node_Resours : Exploration_Node
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void NodeInteract(Exploration_Caravan caravan, SupplyData d)
    {
        base.NodeInteract(caravan,d);
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

            return;
        }

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.AreaSetUp();
    }
}
