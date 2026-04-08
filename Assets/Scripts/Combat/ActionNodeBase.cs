using UnityEngine;
using ADSNameSpace;
using System;


namespace actions
{
    public enum ActionType
    {
        P_MELEE, P_MELEE_Debuff, P_MELEE_Buff, P_MELEE_Status, P_MELEE_Heal,
        P_RANGED, P_RANGED_Debuff, P_RANGED_Buff, P_RANGED_Status, P_RANGED_Heal,
        M_MELEE, M_MELEE_Debuff, M_MELEE_Buff, M_MELEE_Status, M_MELEE_Heal,
        M_RANGED, M_RANGED_Debuff, M_RANGED_Buff, M_RANGED_Status, M_RANGED_Heal,
        MOVEMENT, SummonObject, Knockbacker,
        Self_Buff, Self_Debuff, Self_Status,
    }
    

    [Serializable]
    public struct ActionNodeStats
    {
        public string NodeName;
        public string NodeInfo;
        public float MinScore;
        public int ManaCost;
        public bool IsRoot;
        public effectType Effect;
        public NodeType Node;
        public ActionType ActionType;
        public ActionType[] Reactions;
        public bool IsTeamworkAction;
        public bool TargetAlly;
        public string assetPath;
        public int EnumTarget; //This is used for status/buff/debuff target reactions!
    }

    public abstract class ActionNodeBase : IADSNode<CombatListener, ActionEffectBase>
    {
        protected string nameKey;
        protected string NodeInfo;
        protected ActionEffectBase effect;
        protected bool root = false;
        protected float minimumInputActivationScore;
        protected bool hasInit = false;
        protected DroneUnitBody caster;
        protected string[] possibleParentNameKeys;
        protected ActionType actionType;
        protected ActionType[] reactions;
        protected int manaCost;
        protected int memoryCost;
        protected bool isTeamworkAction;
        protected bool targetAlly;


        public virtual void Init(
            string name,
            string info,
            bool isRoot,
            ActionEffectBase myEffect,
            float minScore,
            int mana,
            ActionType action,
            ActionType[] reaction,
            bool isTeamwork,
            bool TargetAlly
            )
        {
            root = isRoot;
            nameKey = name;
            NodeInfo = info;
            effect = myEffect;
            minimumInputActivationScore = minScore;
            manaCost = mana;
            actionType = action;
            reactions = reaction;
            isTeamworkAction = isTeamwork;
            targetAlly = TargetAlly;
        }
        public virtual void SetupNode(DroneUnitBody myCaster, string[] parents)
        {
            caster = myCaster;
            possibleParentNameKeys = parents;
        }

        public abstract ActionType GetActionType { get; }
        public abstract ActionType[] GetReactionType { get; }

        #region Interface

        public abstract string NameKey { get; }
        public abstract string[] PossibleParentNameKeys { get; }
        public abstract float MinimumInputActivationScore { get; }
        public abstract bool CanBeRoot { get; }
        public abstract CombatListener Input { get; }
        public abstract ActionEffectBase Output { get; }

        public abstract ActionEffectBase GetADSOutput(); //hasInit is used in here!
        public abstract float GetInputScore(CombatListener input);
        public abstract void HandleADSOutputChain(Func<ActionEffectBase> ancestralOutputChain, int maxChainIndex, int chainIndex);
        public abstract void WhenPutOnADSStack(CombatListener input, ActionEffectBase output);

        #endregion
    }
}

