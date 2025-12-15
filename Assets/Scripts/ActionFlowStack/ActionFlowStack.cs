using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ActionFlowStack
{
    public interface IflowAction
    {
        void OnBegin(bool bFirstTime);
        void OnUpdate();
        void OnEnd();
        bool IsDone();
    }

    public static class ActionFlowStackHandler
    {
        private static IflowAction currentAction = null;
        private static Stack<IflowAction> mainActionFlowStack = new Stack<IflowAction>();
        private static HashSet<IflowAction> firstTimersHashSet = new HashSet<IflowAction>();
        private static HashSet<IflowAction> onStackHashSet = new HashSet<IflowAction>();
        private static HashSet<string> callerHashSet = new HashSet<string>();
        private static List<string> mainStackDebug = new List<string>();

        public static HashSet<string> Callers {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return callerHashSet; } }
        public static List<string> MainStackDebug {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return mainStackDebug; } }
        public static IflowAction CurrentFlowAction {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return currentAction; } }

        public static bool PushActionToMainStack(IflowAction newAction)
        {
            // Is the action null?
            if (newAction == null) return false;

            // Is it already on the stack?
            if (onStackHashSet.Contains(newAction) == true) return false;

            // Push new action!
            mainActionFlowStack.Push(newAction);

            // Add to on stack hashSet!
            onStackHashSet.Add(newAction);

            mainStackDebug.Add(newAction.ToString());

            // Set current action to null!
            currentAction = null;

            return true;
        }

        public static bool ReplaceMainStack(IflowAction[] newStack)
        {
            if (newStack == null) return false;

            mainActionFlowStack.Clear();
            onStackHashSet.Clear();
            mainStackDebug.Clear();

            foreach (IflowAction newAction in newStack) PushActionToMainStack(newAction);

            return true;
        }

        // Make calling the update function safer from unconventional calls!
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        public static bool CallUpdateMainActionFlowStack(ref string caller)
        {
            if (callerHashSet.Contains(caller) == false) return false;

            UpdateMainActionFlowStack();

            return true;
        }

        private static void UpdateMainActionFlowStack()
        {
            // Do We have actions?
            if (currentAction == null && mainActionFlowStack.Count == 0) return;

            // New Action?
            while (currentAction == null && mainActionFlowStack.Count > 0)
            {
                // Set the current action!
                currentAction = mainActionFlowStack.Peek();

                // Call on begin!
                bool firstTime = !firstTimersHashSet.Contains(currentAction);
                firstTimersHashSet.Add(currentAction);
                currentAction.OnBegin(firstTime);

                // did OnBegin push or remove another action?
                if (currentAction == null) continue;

                if (mainActionFlowStack.Count > 0 && currentAction != mainActionFlowStack.Peek())
                {
                    currentAction = null;
                    UpdateMainActionFlowStack();
                    return;
                }
            }

            // call OnUpdate 
            if (currentAction == null) return;

            currentAction.OnUpdate();

            // are we still the current action?
            if (mainActionFlowStack.Count > 0 && currentAction == mainActionFlowStack.Peek())
            {
                // are we done?
                if (currentAction.IsDone())
                {
                    mainActionFlowStack.Pop();
                    currentAction.OnEnd();
                    firstTimersHashSet.Remove(currentAction);
                    onStackHashSet.Remove(currentAction);
                    mainStackDebug.Remove(currentAction.ToString());
                    currentAction = null;
                }
            }
            else currentAction = null;
        }
    }
}


