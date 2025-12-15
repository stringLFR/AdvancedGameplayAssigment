using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySquad", menuName = "Squads/Item")]
public class EnemySquadSO : ScriptableObject
{
    [SerializeField]
    private Squad squad;

    public Squad Squad => squad;

}
