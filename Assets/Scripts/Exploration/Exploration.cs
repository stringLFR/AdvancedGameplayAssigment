using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ActionFlowStack;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;
using TMPro;

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

    [SerializeField]
    private Image explorerImage, caravanImage,HostileImage;

    [SerializeField]
    private Canvas canvas;

    public DroneUnitBody Explorer => explorer;

    [SerializeField]
    public DroneUnitBody hostilePrefab;

    [SerializeField]
    public DroneUnitBody caravanPrefab;

    [SerializeField]
    private float managementDistance = 10f, resourceUpkeepTickRate = 1f;

    [SerializeField]
    private GameObject[] nodeResours, nodeSpecial, nodeHazard, nodeTower;

    [SerializeField]
    int nodeResourseAmounts = 10, nodeSpecialAmounts = 10, nodeHazardAmounts = 10, batchAmount = 10;

    [SerializeField]
    private Slider food, intel, mana, medicine, metalics, average;

    [SerializeField]
    private TextMeshProUGUI foodT, intelT, manaT, medicineT, metalicsT, averageT;

    [SerializeField]
    private GameObject startScreen;

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private TextMeshProUGUI gameOverScreenText;

    [SerializeField]
    private Button gameOverScreenButton;

    [SerializeField]
    private TextMeshProUGUI winScoreText;

    public GameObject GameOverScreen => gameOverScreen;
    public TextMeshProUGUI GameOverScreenText => gameOverScreenText;
    public Button GameOverScreenButton => gameOverScreenButton;

    public TextMeshProUGUI WinScoreText => winScoreText;

    public NativeArray<Vector3> positions { get; private set; }
    public NativeArray<Quaternion> rotations { get; private set; }

    JobHandle handle = default;
    JobHandle mainHandle = default;

    float time = 0f;

    public SupplyData[] SupplyData { get; private set; }

    int size;

    const float UIImageHeight = 3f;

    const int maxSupply = 1000;
    const int startingSupply = 500;

    private HashSet<Exploration_Hostile> combatingHostiles = new HashSet<Exploration_Hostile>();
    private List<Exploration_Hostile> defeatedHostiles = new List<Exploration_Hostile>();
    private List<Exploration_Caravan> defeatedCaravans = new List<Exploration_Caravan>();
    private List<Exploration_Node_Tower> activeTowers = new List<Exploration_Node_Tower>();

    public List<Exploration_Caravan> DefeatedCaravans => defeatedCaravans;
    public List<Exploration_Hostile> DefeatedHostiles => defeatedHostiles;

    private List<Image> carvanImages = new List<Image>();
    private List<Image> hostileImages = new List<Image>();

    private Queue<int> towerQueue = new Queue<int>();

    NodeJob_RotateCanvas rotationJob;

    public void SetImagesCaravan(int listSize)
    {
        int size = listSize - carvanImages.Count;

        while (size != 0)
        {
            if (size > 0)
            {
                carvanImages.Add(Instantiate(caravanImage, canvas.transform));
            }
            else if (size < 0)
            {
                Destroy(carvanImages[0].gameObject);
                carvanImages.RemoveAt(0);
            }

            size = listSize - carvanImages.Count;
        }
    }

    public void SetImagesHostile(int listSize)
    {
        int size = listSize - hostileImages.Count;

        while(size != 0)
        {
            if (size > 0)
            {
                hostileImages.Add(Instantiate(HostileImage, canvas.transform));
            }
            else if (size < 0)
            {
                Destroy(hostileImages[0].gameObject);
                hostileImages.RemoveAt(0);
            }

            size = listSize - hostileImages.Count;
        }
    }

    public void AddTowerToQueue(int index)
    {
        towerQueue.Enqueue(index);
    }

    public void SpawnTower(int index, List<Exploration_Node> nodeList)
    {
        if (index > nodeTower.Length) return;

        GameObject obj = Instantiate(nodeTower[index]);

        obj.transform.parent = transform;

        Exploration_Node_Tower r = obj.GetComponent<Exploration_Node_Tower>();

        NavMesh.SamplePosition(explorer.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

        obj.transform.position = hit.position;

        nodeList.Add(r);

        activeTowers.Add(r);
    }

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

            SupplyData[i].MaxAmount = maxSupply;

            SupplyData[i].currentAmount = startingSupply;
        }

        food.maxValue = maxSupply;
        intel.maxValue = maxSupply;
        mana.maxValue = maxSupply;
        medicine.maxValue = maxSupply;
        metalics.maxValue = maxSupply;
        average.maxValue = maxSupply;

        for (int i = 0; i < SupplyData.Length; i++)
        {
            UpdateSlider(SupplyData[i]);
        }

        explorationFlowAction.Init(this);
    }

    public void UpdateSlider(SupplyData data)
    {
        switch (data.Type)
        {
            case SupplyType.FOOD:
                food.value = data.currentAmount;
                foodT.text = $"Food {food.value}/{food.maxValue}";
                break;
            case SupplyType.MANA_STORAGE:
                mana.value = data.currentAmount;
                manaT.text = $"Mana {mana.value}/{mana.maxValue}";
                break;
            case SupplyType.INTEL:
                intel.value = data.currentAmount;
                intelT.text = $"Intel {intel.value}/{intel.maxValue}";
                break;
            case SupplyType.MEDICINE:
                medicine.value = data.currentAmount;
                medicineT.text = $"Medicine {medicine.value}/{medicine.maxValue}";
                break;
            case SupplyType.METALLICS:
                metalics.value = data.currentAmount;
                metalicsT.text = $"Metalics {metalics.value}/{metalics.maxValue}";
                break;
        }

        average.value = (food.value + mana.value + intel.value + medicine.value + metalics.value) / 5;
        averageT.text = $"Average {average.value}/{average.maxValue}";
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
        if (startScreen != null) return;

        ExplorereTick();

        ResourceTick(caravans, hostiles);

        TowerTick(hostiles);

        HostilesTick(hostiles, caravans);

        CaravansTick(caravans);

        NodesTick(nodes);

        MousePointerTick(caravans, hostiles);


        //List<BRT_item<Exploration_Node>> t2 = nodetree.FindClosesItems(2, new System.Numerics.Vector3(explorer.transform.position.x, explorer.transform.position.y, explorer.transform.position.z));
        //print(t2.Count);

        //Debug.DrawLine(explorer.transform.position, new Vector3(t2[0].position.X, t2[0].position.Y, t2[0].position.Z));
        //Debug.DrawLine(explorer.transform.position, new Vector3(t2[1].position.X, t2[1].position.Y, t2[1].position.Z));


    }

    private void ExplorereTick()
    {
        if (Vector3.Dot(Camera.main.transform.forward, explorer.transform.position - Camera.main.transform.position) < 0)
        {
            explorerImage.gameObject.SetActive(false);
        }
        else
        {
            explorerImage.gameObject.SetActive(true);
            explorerImage.transform.position = Camera.main.WorldToScreenPoint(explorer.transform.position + new Vector3(0, UIImageHeight, 0));
        }
    }

    private void ResourceTick(List<Exploration_Caravan> caravans, List<Exploration_Hostile> hostiles)
    {
        time += Time.deltaTime;

        if (time >= resourceUpkeepTickRate)
        {
            time = 0f;

            int enemyAttackers = 0;

            foreach (Exploration_Hostile h in hostiles)
            {
                if (Vector3.Distance(h.body.transform.position, transform.position) <= managementDistance)
                {
                    enemyAttackers++;
                }
            }

            for (int i = 0; i < SupplyData.Length; i++)
            {
                SupplyData[i].currentAmount = Mathf.Clamp(SupplyData[i].currentAmount - ((1 + 1 * caravans.Count) + enemyAttackers * enemyAttackers), 0, SupplyData[i].MaxAmount);

                UpdateSlider(SupplyData[i]);
            }

            if (average.value <= 50)
            {
                FlowAction_GameOver game = new FlowAction_GameOver();
                game.SetGameOverState(false, this);
                ActionFlowStackHandler.PushActionToStack(game);
            }
        }
    }

    private void TowerTick(List<Exploration_Hostile> hostiles)
    {
        float delta = Time.deltaTime;

        foreach (Exploration_Hostile h in hostiles)
        {
            h.ReturnSpeedTick(delta);
        }

        foreach (Exploration_Node_Tower t in activeTowers)
        {
            t.TowerTick(hostiles, delta, this);
        }
    }

    private void HostilesTick(List<Exploration_Hostile> hostiles, List<Exploration_Caravan> caravans)
    {
        int hIndex = 0;
        foreach (Exploration_Hostile h in hostiles)
        {

            if (Vector3.Dot(Camera.main.transform.forward, h.body.transform.position - Camera.main.transform.position) < 0)
            {
                hostileImages[hIndex].gameObject.SetActive(false);
            }
            else
            {
                hostileImages[hIndex].gameObject.SetActive(true);
                hostileImages[hIndex].transform.position = Camera.main.WorldToScreenPoint(h.body.transform.position + new Vector3(0, UIImageHeight, 0));
            }

            hIndex++;

            if (Vector3.Distance(explorer.transform.position, h.GetPosition()) <= 5f)
            {
                if (combatingHostiles.TryGetValue(h, out Exploration_Hostile actualValue) == false)
                {
                    combatingHostiles.Add(h);

                    h.EnterCombat(hostiles, caravans);

                    return;
                }

                switch (ActionFlowStackController.Instance.CombatState)
                {
                    case CombatState.WON:

                        defeatedHostiles.Add(h);

                        ActionFlowStackController.Instance.SetCombatState(CombatState.NOT_IN_COMBAT);
                        break;
                    case CombatState.LOST:

                        combatingHostiles.Remove(h);

                        ActionFlowStackController.Instance.SetCombatState(CombatState.NOT_IN_COMBAT);
                        break;
                    case CombatState.FLEED:

                        combatingHostiles.Remove(h);

                        ActionFlowStackController.Instance.SetCombatState(CombatState.NOT_IN_COMBAT);
                        break;
                }
            }
        }

        if (defeatedHostiles.Count > 0)
        {
            for (int i = 0; i < defeatedHostiles.Count; i++)
            {
                if (combatingHostiles.TryGetValue(defeatedHostiles[i], out Exploration_Hostile actualValue) == true)
                {
                    combatingHostiles.Remove(defeatedHostiles[i]);
                }
                hostiles.Remove(defeatedHostiles[i]);
                Destroy(defeatedHostiles[i].body.gameObject);
                SetImagesHostile(hostiles.Count);

                if (defeatedHostiles[i].node != null)
                {
                    defeatedHostiles[i].node.RemoveOccupier();
                }
            }

            defeatedHostiles.Clear();
        }
    }

    private void CaravansTick(List<Exploration_Caravan> caravans)
    {
        int cIndex = 0;

        foreach (Exploration_Caravan c in caravans)
        {
            if (Vector3.Dot(Camera.main.transform.forward, c.body.transform.position - Camera.main.transform.position) < 0)
            {
                carvanImages[cIndex].gameObject.SetActive(false);
            }
            else
            {
                carvanImages[cIndex].gameObject.SetActive(true);
                carvanImages[cIndex].transform.position = Camera.main.WorldToScreenPoint(c.body.transform.position + new Vector3(0, UIImageHeight, 0));
            }

            cIndex++;

            if (c.goingHome == false)
            {
                if (Vector3.Distance(c.node.transform.position, c.GetPosition()) <= c.node.intereactDistance)
                {
                    c.TransfferSupplies(c.node.IsTaking, null, c.node);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, c.GetPosition()) <= managementDistance)
                {
                    c.TransfferSupplies(c.node.IsTaking, this, null);
                }
            }

            if (c.AmIDead(caravans) == true)
            {
                c.node.RemoveCaravan();
                defeatedCaravans.Add(c);
                c.hunters.Clear();
                continue;
            }

            if (c.node.GetOccupier != null)
            {
                if (c.node.GetOccupier.TriggerHuntForCaravan(c) == true)
                {
                    c.node.RemoveCaravan();
                    defeatedCaravans.Add(c);
                }
            }
        }

        foreach (Exploration_Caravan cDead in defeatedCaravans)
        {
            Destroy(cDead.body.gameObject);
            caravans.Remove(cDead);
            SetImagesCaravan(caravans.Count);
        }

        defeatedCaravans.Clear();
    }

    private void NodesTick(List<Exploration_Node> nodes)
    {
        while(towerQueue.Count > 0)
        {
            int i = towerQueue.Dequeue();

            SpawnTower(i, nodes);
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
    }

    private void MousePointerTick(List<Exploration_Caravan> caravans, List<Exploration_Hostile> hostiles)
    {
        if (MousePoint.instance.IsOverUI == true) return;

        if (Input.GetMouseButtonDown(0))
        {
            NavMesh.SamplePosition(MousePoint.instance.transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            explorer.ProcedualCore.Agent.SetDestination(hit.position);
            explorer.ProcedualCore.ManualNavRotTarget = hit.position;

            if (Vector3.Distance(explorer.transform.position, transform.position) <= managementDistance &&
                Vector3.Distance(transform.position, MousePoint.instance.transform.position) <= managementDistance)
            {
                foreach (Exploration_Caravan c in caravans)
                {
                    c.body.ProcedualCore.Agent.isStopped = true;
                }

                foreach (Exploration_Hostile h in hostiles)
                {
                    h.body.ProcedualCore.Agent.isStopped = true;
                }

                ActionFlowStackHandler.PushActionToStack(new FlowAction_Management { });
            }
        }
    }
}
