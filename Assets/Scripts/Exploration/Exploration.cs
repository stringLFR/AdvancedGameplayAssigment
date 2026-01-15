using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ActionFlowStack;
using Unity.Jobs;
using Unity.Collections;
using System;

public struct NodeJob_RandomPos : IJobFor
{
    public NativeArray<Vector3> positions;
    public Vector3 center;

    public void Execute(int index)
    {
        Unity.Mathematics.Random rand = Unity.Mathematics.Random.CreateFromIndex((uint)index);

        Vector3 randValue = new Vector3()
        {
            x = rand.NextFloat(center.x - 200f, center.x + 200f),
            y = center.y,
            z = rand.NextFloat(center.z - 200f, center.z + 200f),
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

    private BinaryRadianTree<Exploration_Node> nodetree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        FlowAction_Exploration explorationFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Exploration;

        if (explorationFlowAction == null) return;

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

        for (int i = 0; i < size; i++)
        {
            NavMesh.SamplePosition(positions[i], out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            nodes[i].transform.position = hit.position;
            nodes[i].transform.rotation = transform.rotation = Quaternion.FromToRotation(nodes[i].transform.up, hit.normal) * nodes[i].transform.rotation;
        }

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
    }


    // Update is called once per frame
    void Update()
    {

        /*
        testTree = new BinaryRadianTree<DroneUnitBody>(100f, new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));

        List<BRT_item<DroneUnitBody>> t = new List<BRT_item<DroneUnitBody>>();

        t.Add(new BRT_item<DroneUnitBody>(explorer, explorer.transform.position.x, explorer.transform.position.y, explorer.transform.position.z));

        testTree.CreateRadianTree(10, t);

        List<BRT_item<DroneUnitBody>> t2 = testTree.FindClosesItems(1, new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));



        //print(t2[0].position.X);

        */

        
    }
}
