using ActionFlowStack;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Hostile
{
    public DroneUnitBody body { get; private set; }

    private DroneUnitBody explorer;

    int hostileHP;
    float baseSpeed;
    float tempSpeed;

    public float BaseSpeed => baseSpeed;

    public Exploration_Node node { get; private set; }

    public void ReciveTowerEffects(int damage,float speedPenalty, Exploration expo)
    {
        hostileHP -= damage;

        if (speedPenalty < tempSpeed)
        {
            tempSpeed = speedPenalty;
            body.ProcedualCore.Agent.speed = tempSpeed;
        }

        if (hostileHP <= 0)
        {
            expo.DefeatedHostiles.Add(this);
        }
    }

    public void ReturnSpeedTick(float time)
    {
        tempSpeed = Mathf.Clamp(tempSpeed += time,0, baseSpeed);
        body.ProcedualCore.Agent.speed = tempSpeed;
    }

    public Exploration_Hostile(DroneUnitBody explor)
    {
        explorer = explor;
    }

    public Exploration_Hostile SpawnHostile(Exploration expo, Vector3 pos, int hpValue)
    {
        Vector3 randomPointEnemy = pos;
        NavMesh.SamplePosition(randomPointEnemy, out NavMeshHit hitEnemy, Mathf.Infinity, NavMesh.AllAreas);
        GameObject obj = Object.Instantiate(expo.hostilePrefab.gameObject, hitEnemy.position, Quaternion.identity);
        body = obj.GetComponent<DroneUnitBody>();
        hostileHP = hpValue;
        baseSpeed = body.ProcedualCore.Agent.speed;
        tempSpeed = baseSpeed;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public Vector3 GetPosition() => body.transform.position;


    //TODO: Add it so that this can also decide what enemies from the scriptable that will be spawned!
    public void EnterCombat(List<Exploration_Hostile> hostiles, List<Exploration_Caravan> caravans)
    {
        StopExplorationBodies(hostiles, caravans);
        ActionFlowStackHandler.PushActionToStack(new FlowAction_Combat { });
    }

    public static void StopExplorationBodies(List<Exploration_Hostile> hostiles, List<Exploration_Caravan> caravans)
    {
        foreach (Exploration_Hostile h in hostiles)
        {
            h.body.ProcedualCore.Root.tr.gameObject.SetActive(false);
            h.body.ProcedualCore.Agent.isStopped = true;
        }

        foreach (Exploration_Caravan c in caravans)
        {
            c.body.ProcedualCore.Root.tr.gameObject.SetActive(false);
            c.body.ProcedualCore.Agent.isStopped = true;
        }
    }

    public bool TriggerHuntForCaravan(Exploration_Caravan prey)
    {
        if (prey == null || node == null || body == null) return false;

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

    public void Attack(Exploration expo)
    {
        body.ProcedualCore.Agent.SetDestination(expo.transform.position);
        body.ProcedualCore.ManualNavRotTarget = expo.transform.position;
    }

    public void SetHostileDestination(Exploration expo, List<Exploration_Node> nodes, List<Exploration_Caravan> caravans)
    {
        float dist = Vector3.Distance(body.transform.position, expo.transform.position);

        if (dist < 50f || dist > 450f)
        {
            if (node != null)
            {
                node.RemoveOccupier();
                node = null;
            }

            Attack(expo);
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

                    return;
                }
            }

            if (node == null)
            {
                foreach (Exploration_Caravan c in caravans)
                {
                    if (Vector3.Distance(body.transform.position, c.body.transform.position) < 50)
                    {
                        c.hunters.Add(this);

                        body.ProcedualCore.Agent.SetDestination(c.body.transform.position);
                        body.ProcedualCore.ManualNavRotTarget = c.body.transform.position;

                        return;
                    }
                }
            }

            Attack(expo);
        }
    }
}
