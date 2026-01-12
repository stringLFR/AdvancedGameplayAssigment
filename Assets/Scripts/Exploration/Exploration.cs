using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Exploration : MonoBehaviour
{
    [SerializeField]
    private CRPGCamera RPGCamera;
    [SerializeField]
    private DroneUnitBody explorer;

    private BinaryRadianTree<DroneUnitBody> testTree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {

        
        testTree = new BinaryRadianTree<DroneUnitBody>(100f, new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));

        List<BRT_item<DroneUnitBody>> t = new List<BRT_item<DroneUnitBody>>();

        t.Add(new BRT_item<DroneUnitBody>(explorer, explorer.transform.position.x, explorer.transform.position.y, explorer.transform.position.z));

        testTree.CreateRadianTree(10, t);

        List<BRT_item<DroneUnitBody>> t2 = testTree.FindClosesItems(1, new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z));



        //print(t2[0].position.X);

        

        if (MousePoint.instance.IsOverUI == true) return;

        if (Input.GetMouseButtonDown(0))
        {
            NavMesh.SamplePosition(MousePoint.instance.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            explorer.ProcedualCore.Agent.SetDestination(hit.position);
            explorer.ProcedualCore.ManualNavRotTarget = hit.position;
        }
    }
}
