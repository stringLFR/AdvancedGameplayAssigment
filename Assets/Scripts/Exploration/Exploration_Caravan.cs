using UnityEngine;
using UnityEngine.AI;

public class Exploration_Caravan
{
    public DroneUnitBody body { get; private set; }

    public Exploration_Node node { get; private set; }

    public Exploration homeBase { get; private set; }

    public Exploration_Caravan SpawnCaravan(Exploration expo, Exploration_Node targetNode)
    {
        Vector3 randomPointCaravan = expo.transform.position + Random.insideUnitSphere * 10;
        NavMesh.SamplePosition(randomPointCaravan, out NavMeshHit hitcaravan, Mathf.Infinity, NavMesh.AllAreas);
        GameObject obj = Object.Instantiate(expo.hostilePrefab.gameObject, hitcaravan.position, Quaternion.identity);

        //obj.transform.parent = expo.transform;

        body = obj.GetComponent<DroneUnitBody>();

        node = targetNode;

        homeBase = expo;

        return this;
    }
}
