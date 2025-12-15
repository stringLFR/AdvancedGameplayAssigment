using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySquadDataBase", menuName = "SquadDataBase/Item")]
public class EnemySquadDataBaseSO : ScriptableObject
{
    [SerializeField]
    private EnemySquadDatabase[] databases;

    public EnemySquadDatabase[] Databases {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return databases; } }

    [Serializable]
    public struct EnemySquadDatabase
    {
        [SerializeField] private EnemySquadSO[] enemySquads;
        [SerializeField] private int minimumLevelRequirement;

        public EnemySquadSO[] EnemySquads => enemySquads;
        public int MinimumLevelRequirement => minimumLevelRequirement;
    }
}
