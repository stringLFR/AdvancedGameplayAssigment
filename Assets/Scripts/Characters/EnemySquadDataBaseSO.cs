using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySquadDataBase", menuName = "SquadDataBase/Item")]
public class EnemySquadDataBaseSO : ScriptableObject
{
    [SerializeField]
    private EnemySquadDatabase[] databases;

    public EnemySquadDatabase[] Databases { get { return databases; } }

    [Serializable]
    public struct EnemySquadDatabase
    {
        [SerializeField] private EnemySquadSO[] enemySquads;
        [SerializeField] private int minimumLevelRequirement;

        public EnemySquadSO[] EnemySquads => enemySquads;
        public int MinimumLevelRequirement => minimumLevelRequirement;
    }
}
