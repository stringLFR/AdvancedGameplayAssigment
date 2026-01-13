using ActionFlowStack;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Hostile
{
    private DroneUnitBody body;

    public void SpawnHostile(Exploration expo)
    {
        Vector3 randomPointEnemy = Vector3.zero + Random.insideUnitSphere * 50;
        NavMesh.SamplePosition(randomPointEnemy, out NavMeshHit hitEnemy, Mathf.Infinity, NavMesh.AllAreas);
        GameObject obj = Object.Instantiate(expo.hostilePrefab.gameObject, hitEnemy.position, Quaternion.identity);

        body = obj.GetComponent<DroneUnitBody>();
    }

    public Vector3 GetPosition() => body.transform.position;


    //TODO: Add it so that this can also decide what enemies from the scriptable that will be spawned!
    public void EnterCombat()
    {
        ActionFlowStackHandler.PushActionToStack(new FlowAction_Combat { });
    }
}
