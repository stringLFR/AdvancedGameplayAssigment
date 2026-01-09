using UnityEngine;
using ActionFlowStack;
using System.Runtime.CompilerServices;

public sealed class MainMenu : MonoBehaviour
{
    private FlowAction_MainMenu menuFlowAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        menuFlowAction = ActionFlowStack.ActionFlowStackHandler.CurrentFlowAction as FlowAction_MainMenu;

        if (menuFlowAction != null) print("Menu Found!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void StartExploring() => ActionFlowStack.ActionFlowStackHandler.PushActionToStack(new FlowAction_Exploration { });
}
