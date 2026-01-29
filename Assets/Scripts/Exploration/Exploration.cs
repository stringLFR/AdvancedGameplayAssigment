using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ActionFlowStack;
using Unity.Jobs;
using Unity.Collections;
using System;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

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

public struct NodeJob_RotateCanvas : IJobFor
{
    public NativeArray<Vector3> positions;
    public NativeArray<Quaternion> rotations;
    public Vector3 target;

    public void Execute(int index)
    {
        rotations[index] = Quaternion.LookRotation(target - positions[index], Vector3.up);
    }
}

public class Exploration : MonoBehaviour
{
    [SerializeField]
    private CRPGCamera RPGCamera;
    [SerializeField]
    private DroneUnitBody explorer;

    public DroneUnitBody Explorer => explorer;

    [SerializeField]
    public DroneUnitBody hostilePrefab;

    [SerializeField]
    public DroneUnitBody caravanPrefab;

    [SerializeField]
    private float managementDistance = 10f, resourceUpkeepTickRate = 1f;

    [SerializeField]
    private GameObject[] nodeResours, nodeSpecial, nodeHazard;

    [SerializeField]
    int nodeResourseAmounts = 10, nodeSpecialAmounts = 10, nodeHazardAmounts = 10, batchAmount = 10;
    public NativeArray<Vector3> positions { get; private set; }
    public NativeArray<Quaternion> rotations { get; private set; }

    JobHandle handle = default;
    JobHandle mainHandle = default;

    float time = 0f;

    public SupplyData[] SupplyData { get; private set; }

    int size;

    NodeJob_RotateCanvas rotationJob;

    //private BinaryRadianTree<Exploration_Node> nodetree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        FlowAction_Exploration explorationFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Exploration;

        if (explorationFlowAction == null) return;

        //nodetree = new BinaryRadianTree<Exploration_Node>(500f, new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));

        size = nodeResourseAmounts + nodeSpecialAmounts + nodeHazardAmounts;

        positions = new NativeArray<Vector3>(size, Allocator.Persistent);
        rotations = new NativeArray<Quaternion>(size, Allocator.Persistent);

        SupplyData = new SupplyData[5];

        for (int i = 0; i < SupplyData.Length; i++)
        {
            SupplyData[i].Type = (SupplyType)i;

            SupplyData[i].MaxAmount = 1000;

            SupplyData[i].currentAmount = 0;
        }

        explorationFlowAction.Init(this);
    }

    public void MapSetup(List<Exploration_Node> nodeList)
    {

        NodeJob_RandomPos jobData = new NodeJob_RandomPos()
        {
            positions = positions,
            center = transform.position,
            extraRandom = UnityEngine.Random.Range(1, 9999),
        };

        handle = jobData.ScheduleParallel(size, batchAmount, mainHandle);

        List<Exploration_Node> nodes = new List<Exploration_Node>();

        for (int i = 0; i < nodeResourseAmounts; i++)
        {
            GameObject obj = Instantiate(nodeResours[UnityEngine.Random.Range(0, nodeResours.Length - 1)]);

            obj.transform.parent = transform;

            Exploration_Node_Resours r = obj.GetComponent<Exploration_Node_Resours>();

            nodes.Add(r);
        }
        for (int i = 0; i < nodeSpecialAmounts; i++)
        {
            GameObject obj = Instantiate(nodeSpecial[UnityEngine.Random.Range(0, nodeSpecial.Length - 1)]);

            obj.transform.parent = transform;

            Exploration_Node_Special s = obj.GetComponent<Exploration_Node_Special>();

            nodes.Add(s);
        }
        for (int i = 0; i < nodeHazardAmounts; i++)
        {
            GameObject obj = Instantiate(nodeHazard[UnityEngine.Random.Range(0, nodeHazard.Length - 1)]);

            obj.transform.parent = transform;

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
    }


    public void Tick(List<Exploration_Caravan> caravans, List<Exploration_Hostile> hostiles, List<Exploration_Node> nodes)
    {
        time += Time.deltaTime;

        if (time >= resourceUpkeepTickRate)
        {
            time = 0f;

            for (int i = 0; i < SupplyData.Length; i++)
            {
                SupplyData[i].currentAmount = Mathf.Clamp(SupplyData[i].currentAmount - (1 + 1 * caravans.Count) , 0, SupplyData[i].MaxAmount);
            }
        }


        foreach (Exploration_Hostile h in hostiles)
        {
            if (Vector3.Distance(explorer.transform.position, h.GetPosition()) <= 5f)
            {
                h.EnterCombat(hostiles);
            }
        }

        foreach (Exploration_Caravan c in caravans)
        {
            if (c.goingHome == false)
            {
                if (Vector3.Distance(c.node.transform.position, c.GetPosition()) <= c.node.intereactDistance)
                {
                    c.TransfferSupplies(c.node.GivesResources,null, c.node);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, c.GetPosition()) <= managementDistance)
                {
                    c.TransfferSupplies(c.node.GivesResources, this, null);
                }
            }
        }

        foreach (Exploration_Node n in nodes)
        {
            if (Vector3.Distance(Camera.main.transform.position, n.transform.position) >= 100)
            {
                n.Canvas.gameObject.SetActive(false);
                continue;
            }

            n.Canvas.gameObject.SetActive(true);

            n.Canvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - n.Canvas.transform.position, Vector3.up);

            if (Vector3.Distance(explorer.transform.position, n.transform.position) <= n.intereactDistance)
            {
                if (n.assignedCaravan == false)
                {
                    n.Canvas.Button.gameObject.SetActive(true);
                }
                else
                {
                    n.Canvas.Button.gameObject.SetActive(false);
                }
            }
            else
            {
                n.Canvas.Button.gameObject.SetActive(false);
            }
        }

        if (MousePoint.instance.IsOverUI == true) return;

        if (Input.GetMouseButtonDown(0))
        {
            NavMesh.SamplePosition(MousePoint.instance.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            explorer.ProcedualCore.Agent.SetDestination(hit.position);
            explorer.ProcedualCore.ManualNavRotTarget = hit.position;

            if (Vector3.Distance(explorer.transform.position, transform.position) <= managementDistance &&
                Vector3.Distance(transform.position, MousePoint.instance.transform.position) <= managementDistance)
            {
                handle.Complete();
                ActionFlowStackHandler.PushActionToStack(new FlowAction_Management { });
            }
        }


        //List<BRT_item<Exploration_Node>> t2 = nodetree.FindClosesItems(2, new System.Numerics.Vector3(explorer.transform.position.x, explorer.transform.position.y, explorer.transform.position.z));
        //print(t2.Count);

        //Debug.DrawLine(explorer.transform.position, new Vector3(t2[0].position.X, t2[0].position.Y, t2[0].position.Z));
        //Debug.DrawLine(explorer.transform.position, new Vector3(t2[1].position.X, t2[1].position.Y, t2[1].position.Z));


    }

}
