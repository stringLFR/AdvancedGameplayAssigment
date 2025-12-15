using System;
using System.Collections.Generic;
using System.Linq;

// Tips:
// ADS do not need to be used/called every frame! One call per second as an example!
// Input and output should be able to handle themselves!
// They just recive orders from ADS and continue on their own with said feedback/order/command.
// Input and output could themselves carry different versions of ADS in them for different categories!
// Example: Main ADS = main AI. Secondary ADS in output/input = movement/shooting etc.
// can can inherit from main ADS class,
// but for the most part you only really need to think about the interface! 
// ADS main goal is decision making! So it does not need to be used for AI!
// (You can of course if you want to!)

namespace ADSNameSpace // ADS = Ancestral Decision stack
{
    // I = input class/struct/object. O = output class/struct/object.
    public interface IADSNode<I,O> 
    {
        public string NameKey { get; }
        public string[] PossibleParentNameKeys { get; }
        public float MinimumInputActivationScore { get; }
        public bool CanBeRoot { get; }
        public I Input { get; }
        public O Output { get; }

        //https://learn.microsoft.com/en-us/dotnet/api/system.delegate.getinvocationlist?view=net-9.0
        //Example!
        //float printValue = 0f;
        //foreach (Func<float> func in ancestralOutputChain.GetInvocationList())
        //{
        //  printValue += func();
        //}

        //This method gets the output chain together with both the max and current index!
        //The func delegates are the GetADSOutput() methods!
        public void HandleADSOutputChain(Func<O> ancestralOutputChain, int maxChainIndex, int chainIndex);

        //Returns a float based on what the input says.
        //If input is a float, then just return that! (Modified or not)
        public float GetInputScore(I input);

        //This only happens if this node gets put on the stack!
        public void WhenPutOnADSStack(I input, O output);

        //public O GetADSOutput() => Output; You can do this if
        //you want/don't need extra stuff in GetADSOutput()!
        public O GetADSOutput(); 
    }

    public interface IADSCreator<I,O>
    {
        //In case you need more functionallity when creating ADSs!
        public ADS<I, O> CreateADS(int maxStackCount, int maxADSNodeCappacity);

        public ADS<I, O> MyADS { get; } //Returns this creators ADS!
    }

    public abstract class ADSOutput_Abstract
    {

    }

    public abstract class ADSInput_Abstract
    {

    }

    public class ADS<I,O> 
    {
        protected Func<O> ancestralOutputChain;
        protected Dictionary<string,IADSNode<I,O>> ADSNodes;
        protected Stack<string> ADSNodesNameKeyStack;
        protected string[] allNameKeys;
        protected readonly int maxCount;
        protected readonly int maxADSNodeCappacity;
        protected int TotalADSNodes = 0;

        public ADS(int newMaxCount, int newMaxDecisionCappacity)
        {
            ADSNodes = new Dictionary<string, IADSNode<I, O>>();
            ADSNodesNameKeyStack = new Stack<string>();
            maxCount = newMaxCount;
            maxADSNodeCappacity = newMaxDecisionCappacity;
            allNameKeys = new string[maxADSNodeCappacity];
        }

        public virtual bool AddADSNode(IADSNode<I,O> ADS)
        {
            if (TotalADSNodes >= maxADSNodeCappacity) return false;

            if (allNameKeys.Contains(ADS.NameKey) == true) return false;

            ADSNodes.Add(ADS.NameKey, ADS);
            allNameKeys[TotalADSNodes] = ADS.NameKey;
            TotalADSNodes++;
            return true;
        }

        public virtual bool RemoveADSNode(string keyName)
        {
            ADSNodes.Remove(keyName);

            for (int i = 0; i < allNameKeys.Length; i++)
            {
                if (allNameKeys[i] != keyName) continue;

                allNameKeys[i] = null;
                TotalADSNodes--;
                return true;
            }

            return false;
        }

        public virtual void ActivateAncestryChain()
        {
            Stack<string> cleanUpStack = new Stack<string>();
            int maxIndex = ADSNodesNameKeyStack.Count - 1;

            while (ADSNodesNameKeyStack.Count > 0)
            {
                string currentNode = ADSNodesNameKeyStack.Pop();
                int chainIndex = (maxIndex - ADSNodesNameKeyStack.Count);
                ancestralOutputChain += ADSNodes[currentNode].GetADSOutput;
                ADSNodes[currentNode].HandleADSOutputChain(ancestralOutputChain, maxIndex, chainIndex);
                cleanUpStack.Push(currentNode);
            }

            while (cleanUpStack.Count > 0)
            {
                string currentNode = cleanUpStack.Pop();
                ancestralOutputChain -= ADSNodes[currentNode].GetADSOutput;
            }
        }

        public virtual int CreateAncestryStack() //Returns the count of stack!
        {
            ADSNodesNameKeyStack.Clear();

            return GetNextADSNodeDecision(allNameKeys, TotalADSNodes);

            int GetNextADSNodeDecision(string[] arr, int arrLength)
            {
                if (ADSNodesNameKeyStack.Count >= maxCount) return ADSNodesNameKeyStack.Count;

                float currentBestScore = 0;
                string currentBestDecision = null;

                for (int i = 0; i < arrLength; i++)
                {
                    if (ADSNodes.TryGetValue(arr[i], out IADSNode<I, O> result) == true)
                    {
                        if (ADSNodesNameKeyStack.Count == 0 && result.CanBeRoot == false) continue;

                        float score = result.GetInputScore(ADSNodes[arr[i]].Input);

                        if (score < result.MinimumInputActivationScore) continue;

                        if (score > currentBestScore)
                        {
                            currentBestScore = score;
                            currentBestDecision = arr[i];
                        }
                    }
                }

                if (currentBestDecision == null) return ADSNodesNameKeyStack.Count;
                else
                {
                    ADSNodesNameKeyStack.Push(ADSNodes[currentBestDecision].NameKey);

                    ADSNodes[ADSNodesNameKeyStack.Peek()].WhenPutOnADSStack(
                        ADSNodes[ADSNodesNameKeyStack.Peek()].Input, 
                        ADSNodes[ADSNodesNameKeyStack.Peek()].Output);

                    return GetNextADSNodeDecision(ADSNodes[ADSNodesNameKeyStack.Peek()].PossibleParentNameKeys,
                    ADSNodes[ADSNodesNameKeyStack.Peek()].PossibleParentNameKeys.Length);
                }
            }
        }
    }
}
