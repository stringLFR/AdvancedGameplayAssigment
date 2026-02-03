using ActionFlowStack;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Hostile
{
    public DroneUnitBody body { get; private set; }

    private DroneUnitBody explorer;

    public Exploration_Hostile(DroneUnitBody explor)
    {
        explorer = explor;
    }

    public void SpawnHostile(Exploration expo)
    {
        Vector3 randomPointEnemy = expo.transform.position + Random.insideUnitSphere * 500;
        NavMesh.SamplePosition(randomPointEnemy, out NavMeshHit hitEnemy, Mathf.Infinity, NavMesh.AllAreas);
        GameObject obj = Object.Instantiate(expo.hostilePrefab.gameObject, hitEnemy.position, Quaternion.identity);

        //obj.transform.parent = expo.transform;

        body = obj.GetComponent<DroneUnitBody>();
    }

    public Vector3 GetPosition() => body.transform.position;


    //TODO: Add it so that this can also decide what enemies from the scriptable that will be spawned!
    public void EnterCombat(List<Exploration_Hostile> hostiles, List<Exploration_Caravan> caravans)
    {
        foreach(Exploration_Hostile h in hostiles)
        {
            h.body.gameObject.SetActive(false);
        }

        foreach (Exploration_Caravan c in caravans)
        {
            c.body.gameObject.SetActive(false);
        }
        ActionFlowStackHandler.PushActionToStack(new FlowAction_Combat { });
    }
}
