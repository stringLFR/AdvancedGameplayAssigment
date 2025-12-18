using UnityEngine;
using UnityEngine.AI;

public class Exploration : MonoBehaviour
{
    [SerializeField]
    private CRPGCamera RPGCamera;
    [SerializeField]
    private DroneUnitBody explorer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MousePoint.instance.IsOverUI == true) return;

        if (Input.GetMouseButtonDown(0))
        {
            NavMesh.SamplePosition(MousePoint.instance.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            explorer.ProcedualCore.Agent.SetDestination(hit.position);
            explorer.ProcedualCore.ManualNavRotTarget = hit.position;
        }
    }
}
