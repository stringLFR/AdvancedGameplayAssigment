using actions;
using UnityEngine;

[CreateAssetMenu(fileName = "ReactionSO", menuName = "Scriptable Objects/ReactionSO")]
public class ReactionSO : ScriptableObject
{
    [SerializeField]
    public ActionNodeStats reactionStats;
}
