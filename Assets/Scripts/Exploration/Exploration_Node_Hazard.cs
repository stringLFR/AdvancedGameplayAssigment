using UnityEngine;
using UnityEngine.AI;

public class Exploration_Node_Hazard : Exploration_Node
{
    [SerializeField]
    protected NavMeshObstacle NavMeshObstacle;

    public override void NodeInteract(Exploration_Caravan caravan)
    {
        throw new System.NotImplementedException();
    }

    public override void NodeInteract()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float size = AreaSetUp();

        NavMeshObstacle.radius *= size;
        NavMeshObstacle.height *= size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
