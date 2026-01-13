using ActionFlowStack;
using UnityEngine;

public class Exploration_Management : MonoBehaviour
{
    FlowAction_Management managementFlowAction;

    private void Awake()
    {
        managementFlowAction = ActionFlowStackHandler.CurrentFlowAction as FlowAction_Management;

        if (managementFlowAction == null) return;

        managementFlowAction.Init(this);
    }

    public void CloseManagement()
    {
        if (managementFlowAction == null) return;

        managementFlowAction.GoExploring();
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
