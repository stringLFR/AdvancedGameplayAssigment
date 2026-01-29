using UnityEngine;

public class Exploration_Node_Special : Exploration_Node
{
    public override void NodeInteract(Exploration_Caravan caravan)
    {
        base.NodeInteract(caravan);
    }

    public override void NodeInteract()
    {
        base.NodeInteract();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AreaSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
