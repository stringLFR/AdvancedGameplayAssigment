using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Caravan
{
    public DroneUnitBody body { get; private set; }

    public Exploration_Node node { get; private set; }

    public Exploration homeBase { get; private set; }

    public List<Exploration_Hostile> hunters { get; private set; }

    private SupplyData[] caravanData;

    public bool goingHome { get; private set; }

    private Vector3 nodeTargetPos = Vector3.zero;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public Vector3 GetPosition() => body.transform.position;

    //Currently it is possible to lose resources if the current amount is equal to max!
    public void TransfferSupplies(bool isTaking,Exploration expo = null, Exploration_Node leNode = null)
    {
        if (expo == null && leNode == null) return;

        if (expo != null)
        {
            int totalFeed = 0;

            for (int i = 0; i < caravanData.Length; i++)
            {
                for(int j = 0; j < expo.SupplyData.Length; j++)
                {
                    if (caravanData[i].Type != expo.SupplyData[j].Type) continue;

                    if (isTaking == true && caravanData[i].currentAmount == caravanData[i].MaxAmount)
                    {
                        totalFeed++;
                        continue;
                    }

                    int initialAmount = expo.SupplyData[j].currentAmount;

                    int newAmount = Mathf.Clamp(
                        isTaking == true ?
                        expo.SupplyData[j].currentAmount - caravanData[i].MaxAmount : 
                        expo.SupplyData[j].currentAmount + caravanData[i].currentAmount,
                        0, expo.SupplyData[j].MaxAmount);

                    int transfferAmount = isTaking == true ? initialAmount - newAmount : caravanData[i].currentAmount - caravanData[i].currentAmount;

                    expo.SupplyData[j].currentAmount = newAmount;

                    expo.UpdateSlider(expo.SupplyData[j]);

                    caravanData[i].currentAmount = transfferAmount;
                }
            }

            if (node == null || totalFeed == node.Supplies.Length) return;

            goingHome = false;

            body.ProcedualCore.Agent.SetDestination(nodeTargetPos);
            body.ProcedualCore.ManualNavRotTarget = node.transform.position;

            return;
        }

        if (leNode != null)
        {
            int totalEmpty = 0;
            int totalFeed = 0;

            for (int i = 0; i < caravanData.Length; i++)
            {
                for (int j = 0; j < leNode.Supplies.Length; j++)
                {
                    if (caravanData[i].Type != leNode.Supplies[j].Type) continue;

                    if (isTaking == true && leNode.Supplies[j].currentAmount == leNode.Supplies[j].MaxAmount)
                    {
                        totalFeed++;
                        continue;
                    }

                    int initialAmount = leNode.Supplies[j].currentAmount;

                    int newAmount = Mathf.Clamp(
                        isTaking == true ?
                        leNode.Supplies[j].currentAmount + caravanData[i].currentAmount :
                        leNode.Supplies[j].currentAmount - caravanData[i].MaxAmount,
                        0, leNode.Supplies[j].MaxAmount);

                    int transfferAmount = isTaking == true ? caravanData[i].currentAmount - caravanData[i].currentAmount : initialAmount - newAmount;

                    leNode.Supplies[j].currentAmount = newAmount;

                    if (isTaking == false && leNode.Supplies[j].currentAmount <= 0) totalEmpty++;

                    leNode.NodeInteract(this, leNode.Supplies[j]);

                    caravanData[i].currentAmount = transfferAmount;
                }
            }

            if (totalEmpty == leNode.Supplies.Length || totalFeed == leNode.Supplies.Length)
            {
                node.OnFeedOrEmpty(isTaking,this,homeBase);

                return;
            }

            goingHome = true;

            body.ProcedualCore.Agent.SetDestination(homeBase.transform.position);
            body.ProcedualCore.ManualNavRotTarget = homeBase.transform.position;

            return;
        }
    }

    public bool AmIDead(List<Exploration_Caravan> caravans)
    {
        foreach (Exploration_Hostile item in hunters)
        {
            if (Vector3.Distance(item.body.transform.position, body.transform.position) <= 5)
            {
                //Will allow it to pick the last caravan again!
                Exploration_Caravan newTarget = caravans[UnityEngine.Random.Range(0, caravans.Count - 1)];

                item.body.ProcedualCore.Agent.SetDestination(newTarget.body.transform.position);
                item.body.ProcedualCore.ManualNavRotTarget = newTarget.body.transform.position;
                newTarget.hunters.Add(item);

                return true;
            }
        }
        return false;
    }

    public Exploration_Caravan SpawnCaravan(Exploration expo, Exploration_Node targetNode)
    {
        Vector3 randomPointCaravan = expo.transform.position + Random.insideUnitSphere * 10;
        NavMesh.SamplePosition(randomPointCaravan, out NavMeshHit hitcaravan, Mathf.Infinity, NavMesh.AllAreas);
        GameObject obj = Object.Instantiate(expo.hostilePrefab.gameObject, hitcaravan.position, Quaternion.identity);

        //obj.transform.parent = expo.transform;

        body = obj.GetComponent<DroneUnitBody>();

        node = targetNode;

        homeBase = expo;

        hunters = new List<Exploration_Hostile>();

        caravanData = new SupplyData[targetNode.Supplies.Length];

        for (int i = 0; i < caravanData.Length; i++)
        {
            caravanData[i].MaxAmount = targetNode.Supplies[i].carryAmount;
            caravanData[i].Type = targetNode.Supplies[i].Type;
        }

        goingHome = false;

        nodeTargetPos = homeBase.Explorer.transform.position;

        body.ProcedualCore.Agent.SetDestination(nodeTargetPos);
        body.ProcedualCore.ManualNavRotTarget = node.transform.position;

        return this;
    }
}
