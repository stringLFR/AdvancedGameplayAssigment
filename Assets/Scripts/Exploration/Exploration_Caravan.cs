using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Exploration_Caravan
{
    public DroneUnitBody body { get; private set; }

    public Exploration_Node node { get; private set; }

    public Exploration homeBase { get; private set; }

    private SupplyData[] caravanData;

    public bool goingHome { get; private set; }

    public Vector3 GetPosition() => body.transform.position;

    //Currently it is possible to lose resources if the current amount is equal to max!
    public void TransfferSupplies(bool isTaking,Exploration expo = null, Exploration_Node leNode = null)
    {
        if (expo == null && leNode == null) return;

        if (expo != null)
        {
            for (int i = 0; i < caravanData.Length; i++)
            {
                for(int j = 0; j < expo.SupplyData.Length; j++)
                {
                    if (caravanData[i].Type != expo.SupplyData[j].Type) continue;

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

            goingHome = false;

            body.ProcedualCore.Agent.SetDestination(node.transform.position);
            body.ProcedualCore.ManualNavRotTarget = node.transform.position;

            return;
        }

        if (leNode != null)
        {
            for (int i = 0; i < caravanData.Length; i++)
            {
                for (int j = 0; j < leNode.Supplies.Length; j++)
                {
                    if (caravanData[i].Type != leNode.Supplies[j].Type) continue;

                    int initialAmount = leNode.Supplies[j].currentAmount;

                    int newAmount = Mathf.Clamp(
                        isTaking == true ?
                        leNode.Supplies[j].currentAmount + caravanData[i].currentAmount :
                        leNode.Supplies[j].currentAmount - caravanData[i].MaxAmount,
                        0, leNode.Supplies[j].MaxAmount);

                    int transfferAmount = isTaking == true ? caravanData[i].currentAmount - caravanData[i].currentAmount : initialAmount - newAmount;

                    leNode.Supplies[j].currentAmount = newAmount;

                    leNode.NodeInteract(this, leNode.Supplies[j]);

                    caravanData[i].currentAmount = transfferAmount;
                }
            }

            goingHome = true;

            body.ProcedualCore.Agent.SetDestination(homeBase.transform.position);
            body.ProcedualCore.ManualNavRotTarget = homeBase.transform.position;

            return;
        }
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

        caravanData = new SupplyData[targetNode.Supplies.Length];

        for (int i = 0; i < caravanData.Length; i++)
        {
            caravanData[i].MaxAmount = targetNode.Supplies[i].carryAmount;
            caravanData[i].Type = targetNode.Supplies[i].Type;
        }

        goingHome = false;

        body.ProcedualCore.Agent.SetDestination(node.transform.position);
        body.ProcedualCore.ManualNavRotTarget = node.transform.position;

        return this;
    }
}
