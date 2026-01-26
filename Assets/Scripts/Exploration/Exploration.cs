using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ActionFlowStack;
using Unity.Jobs;
using Unity.Collections;

public struct NodeJob_RandomPos : IJobFor
{
    public NativeArray<Vector3> positions;
    public Vector3 center;
    public int extraRandom;

    public void Execute(int index)
    {
        Unity.Mathematics.Random rand = Unity.Mathematics.Random.CreateFromIndex((uint)(index + extraRandom));

        Vector3 randValue = new Vector3()
        {
            x = rand.NextFloat(center.x - 250f, center.x + 250f),
            y = center.y,
            z = rand.NextFloat(center.z - 250f, center.z + 250f),
        };

        positions[index] = randValue;
    }
}

public class Exploration : MonoBehaviour
{
    [SerializeField]
    private CRPGCamera RPGCamera;
    [SerializeField]
    private DroneUnitBody explorer;

    [SerializeField]
    public DroneUnitBody hostilePrefab;

    [SerializeField]
    public DroneUnitBody caravanPrefab;

    [SerializeField]
    private float managementDistance = 10f;

    [SerializeField]
    private GameObject nodeResours, nodeSpecial, nodeHazard;

    [SerializeField]
    int nodeResourseAmounts = 10, nodeSpecialAmounts = 10, nodeHazardAmounts = 10, batchAmount = 10;

    //private BinaryRadianTree<Exploration_Node> nodetree;

    public List<Exploration_Node> nodeList { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        FlowAction_Exploration explorationFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Exploration;

        if (explorationFlowAction == null) return;

        nodeList = new List<Exploration_Node>();

        //nodetree = new BinaryRadianTree<Exploration_Node>(500f, new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));

        explorationFlowAction.Init(this);
    }

    public void MapSetup()
    {
        int size = nodeResourseAmounts + nodeSpecialAmounts + nodeHazardAmounts;

        NativeArray<Vector3> positions = new NativeArray<Vector3>(size, Allocator.Persistent);


        NodeJob_RandomPos jobData = new NodeJob_RandomPos()
        {
            positions = positions,
            center = transform.position,
            extraRandom = UnityEngine.Random.Range(1, 9999),
        };

        JobHandle handle = default;
        JobHandle mainHandle = default;

        handle = jobData.ScheduleParallel(size, batchAmount, mainHandle);

        List<Exploration_Node> nodes = new List<Exploration_Node>();

        for (int i = 0; i < nodeResourseAmounts; i++)
        {
            GameObject obj = Instantiate(nodeResours);

            Exploration_Node_Resours r = obj.GetComponent<Exploration_Node_Resours>();

            nodes.Add(r);
        }
        for (int i = 0; i < nodeSpecialAmounts; i++)
        {
            GameObject obj = Instantiate(nodeSpecial);

            Exploration_Node_Special s = obj.GetComponent<Exploration_Node_Special>();

            nodes.Add(s);
        }
        for (int i = 0; i < nodeHazardAmounts; i++)
        {
            GameObject obj = Instantiate(nodeHazard);

            Exploration_Node_Hazard h = obj.GetComponent<Exploration_Node_Hazard>();

            nodes.Add(h);
        }

        handle.Complete();

        
        //List<BRT_item<Exploration_Node>> list = new List<BRT_item<Exploration_Node>>();

        for (int i = 0; i < size; i++)
        {
            NavMesh.SamplePosition(positions[i], out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            nodes[i].transform.position = hit.position;
            //nodes[i].transform.localRotation = Quaternion.FromToRotation(nodes[i].transform.up, hit.normal);

            nodeList.Add(nodes[i]);
            //list.Add(new BRT_item<Exploration_Node>(nodes[i], nodes[i].transform.position.x, nodes[i].transform.position.y, nodes[i].transform.position.z));
        }

        nodeList.Sort((x, y) => Vector3.Distance(x.transform.position, transform.position).CompareTo(Vector3.Distance(y.transform.position, transform.position)));

        //nodetree.CreateRadianTree(10, list);

        positions.Dispose();
    }


    public void Tick(List<Exploration_Caravan> caravans, List<Exploration_Hostile> hostiles, List<Exploration_Node> nodes)
    {
        if (MousePoint.instance.IsOverUI == true) return;

        if (Input.GetMouseButtonDown(0))
        {
            NavMesh.SamplePosition(MousePoint.instance.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            explorer.ProcedualCore.Agent.SetDestination(hit.position);
            explorer.ProcedualCore.ManualNavRotTarget = hit.position;

            if (Vector3.Distance(explorer.transform.position, transform.position) <= managementDistance && 
                Vector3.Distance(transform.position, MousePoint.instance.transform.position) <= managementDistance)
            {
                ActionFlowStackHandler.PushActionToStack(new FlowAction_Management { });
            }
        }

        if (Vector3.Distance(explorer.transform.position, hostiles[0].GetPosition()) <= 10f)
        {
            hostiles[0].EnterCombat();
        }

        //List<BRT_item<Exploration_Node>> t2 = nodetree.FindClosesItems(2, new System.Numerics.Vector3(explorer.transform.position.x, explorer.transform.position.y, explorer.transform.position.z));
        //print(t2.Count);

        //Debug.DrawLine(explorer.transform.position, new Vector3(t2[0].position.X, t2[0].position.Y, t2[0].position.Z));
        //Debug.DrawLine(explorer.transform.position, new Vector3(t2[1].position.X, t2[1].position.Y, t2[1].position.Z));


    }

}
