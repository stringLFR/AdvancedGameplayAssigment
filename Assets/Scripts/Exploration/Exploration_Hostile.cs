using ActionFlowStack;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Hostile
{
    public DroneUnitBody body { get; private set; }

    private DroneUnitBody explorer;

    public Exploration_Node node { get; private set; }

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
            h.body.ProcedualCore.Root.tr.gameObject.SetActive(false);
        }

        foreach (Exploration_Caravan c in caravans)
        {
            c.body.ProcedualCore.Root.tr.gameObject.SetActive(false);
        }
        ActionFlowStackHandler.PushActionToStack(new FlowAction_Combat { });
    }

    public bool TriggerHuntForCaravan(Exploration_Caravan prey)
    {
        if (Vector3.Distance(prey.body.transform.position, body.transform.position) <= node.intereactDistance * 2)
        {
            body.ProcedualCore.Agent.SetDestination(prey.body.transform.position);
            body.ProcedualCore.ManualNavRotTarget = prey.body.transform.position;
        }

        if (Vector3.Distance(prey.body.transform.position, body.transform.position) <= 5)
        {
            body.ProcedualCore.Agent.SetDestination(node.transform.position);
            body.ProcedualCore.ManualNavRotTarget = node.transform.position;
            return true;
        }

        return false;
    }

    public void SetHostileDestination(Exploration expo, List<Exploration_Node> nodes)
    {
        if (Vector3.Distance(body.transform.position, expo.transform.position) <= 50)
        {
            body.ProcedualCore.Agent.SetDestination(expo.transform.position);
            body.ProcedualCore.ManualNavRotTarget = expo.transform.position;
        }
        else
        {
            foreach(Exploration_Node n in nodes)
            {
                if (n is Exploration_Node_Hazard) continue;

                if (n.GetOccupier != null) continue;

                float rand = UnityEngine.Random.Range(0f, 1f);

                if (rand >= 0.5f)
                {
                    body.ProcedualCore.Agent.SetDestination(n.transform.position);
                    body.ProcedualCore.ManualNavRotTarget = n.transform.position;

                    node = n;
                    node.AddOccupier(this);

                    break;
                }
            }
        }
    }
}
