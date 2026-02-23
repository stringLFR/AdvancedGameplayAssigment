using ActionFlowStack;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Exploration_Management : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradePage;

    [SerializeField]
    private DroneUpgradePage[] droneUpgradePages;

    FlowAction_Management managementFlowAction;

    private void Awake()
    {
        managementFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Management;

        if (managementFlowAction == null) return;

        managementFlowAction.Init(this, FindFirstObjectByType<Exploration>());

        int index = -1;

        foreach (DroneUpgradePage upgradePage in droneUpgradePages)
        {
            index++;

            if (index > ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits.Length - 1)
            {
                upgradePage.gameObject.SetActive(false);
                continue;
            }

            upgradePage.Init(this, ActionFlowStackController.Instance.Team.PlayerMembers.droneUnits[index]);
        }

        upgradePage.SetActive(false);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void CloseManagement()
    {
        if (managementFlowAction == null) return;

        managementFlowAction.GoExploring();
    }

    public void OpenUpgradePage()
    {
        upgradePage.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
