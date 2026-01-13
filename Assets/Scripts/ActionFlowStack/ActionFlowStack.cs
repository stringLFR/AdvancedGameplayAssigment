using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ActionFlowStack
{
    public interface IflowAction
    {
        void OnBegin(bool bFirstTime);
        void OnUpdate();
        void OnEnd();
        bool IsDone();
    }

    public class ActionFlowStackObject
    {
        private IflowAction currentAction = null;
        private Stack<IflowAction> actionFlowStack = new Stack<IflowAction>();
        private HashSet<IflowAction> firstTimersHashSet = new HashSet<IflowAction>();
        private HashSet<IflowAction> onStackHashSet = new HashSet<IflowAction>();

        public IflowAction CurrentAction { get { return currentAction; }  set { currentAction = value; } }
        public Stack<IflowAction> ActionFlowStack => actionFlowStack;
        public HashSet<IflowAction> FirstTimersHashSet => firstTimersHashSet;
        public HashSet<IflowAction> OnStackHashSet => onStackHashSet;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        public void UpdateThisActionStack() => ActionFlowStackHandler.UpdateNonStaticActionStack(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        public void PushActionToThisStack(IflowAction newAction) => ActionFlowStackHandler.PushActionToStack(newAction, this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        public void ReplaceThisActionStack(IflowAction[] newStack) => ActionFlowStackHandler.ReplaceStack(newStack, this);

    }

    public static class ActionFlowStackHandler
    {
        private static IflowAction currentAction = null;
        private static Stack<IflowAction> mainActionFlowStack = new Stack<IflowAction>();
        private static HashSet<IflowAction> firstTimersHashSet = new HashSet<IflowAction>();
        private static HashSet<IflowAction> onStackHashSet = new HashSet<IflowAction>();
        private static HashSet<string> callerHashSet = new HashSet<string>();

        public static HashSet<string> Callers {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return callerHashSet; } }

        public static IflowAction CurrentFlowAction {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return currentAction; } }

        public static bool PushActionToStack(IflowAction newAction, ActionFlowStackObject nonStatic = null)
        {
            // Is the action null?
            if (newAction == null) return false;

            HashSet<IflowAction> onHash = nonStatic == null ? onStackHashSet : nonStatic.OnStackHashSet;
            Stack<IflowAction> theStack = nonStatic == null ? mainActionFlowStack : nonStatic.ActionFlowStack;

            // Is it already on the stack?
            if (onHash.Contains(newAction) == true) return false;

            // Push new action!
            theStack.Push(newAction);

            // Add to on stack hashSet!
            onHash.Add(newAction);

            // Set current action to null!
            IflowAction action = nonStatic == null ? currentAction = null: nonStatic.CurrentAction = null;

            return true;
        }

        public static bool ReplaceStack(IflowAction[] newStack, ActionFlowStackObject nonStatic = null)
        {
            if (newStack == null) return false;

            HashSet<IflowAction> onHash = nonStatic == null ? onStackHashSet : nonStatic.OnStackHashSet;
            Stack<IflowAction> theStack = nonStatic == null ? mainActionFlowStack : nonStatic.ActionFlowStack;

            while (theStack.Count < 1)
            {
                theStack.Peek().OnEnd();
                theStack.Pop();
            }

            onHash.Clear();

            foreach (IflowAction newAction in newStack) PushActionToStack(newAction, nonStatic);

            return true;
        }

        // Make calling the update function safer from unconventional calls!
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        public static bool CallUpdateMainActionFlowStack(ref string caller)
        {
            if (callerHashSet.Contains(caller) == false) return false;

            UpdateActionFlowStack();

            return true;
        }

        public static void UpdateNonStaticActionStack(ActionFlowStackObject nonStatic)
        {
            if (nonStatic == null) return;

            UpdateActionFlowStack(nonStatic);
        }

        private static void UpdateActionFlowStack(ActionFlowStackObject nonStatic = null)
        {
            HashSet<IflowAction> onHash = nonStatic == null ? onStackHashSet : nonStatic.OnStackHashSet;
            HashSet<IflowAction> firstTimer = nonStatic == null ? firstTimersHashSet : nonStatic.FirstTimersHashSet;
            Stack<IflowAction> theStack = nonStatic == null ? mainActionFlowStack : nonStatic.ActionFlowStack;
            IflowAction action = nonStatic == null ? currentAction : nonStatic.CurrentAction;

            // Do We have actions?
            if (action == null && theStack.Count == 0) return;

            // New Action?
            while (action == null && theStack.Count > 0)
            {
                // Set the current action!
                action = nonStatic == null ? currentAction = theStack.Peek() : nonStatic.CurrentAction = theStack.Peek();

                // Call on begin!
                bool firstTime = !firstTimer.Contains(action);
                firstTimer.Add(action);
                action.OnBegin(firstTime);

                // did OnBegin push or remove another action?
                if (action == null) continue;

                if (theStack.Count > 0 && action != theStack.Peek())
                {
                    action = nonStatic == null ? currentAction = null : nonStatic.CurrentAction = null;
                    UpdateActionFlowStack(nonStatic);
                    return;
                }
            }

            // call OnUpdate 
            if (action == null) return;

            action.OnUpdate();

            // are we still the current action?
            if (theStack.Count > 0 && action == theStack.Peek())
            {
                // are we done?
                if (action.IsDone())
                {
                    theStack.Pop();
                    action.OnEnd();
                    firstTimer.Remove(action);
                    onHash.Remove(action);
                    action = nonStatic == null ? currentAction = null : nonStatic.CurrentAction = null;
                }
            }
            else action = nonStatic == null ? currentAction = null : nonStatic.CurrentAction = null;
        }
    }
}


