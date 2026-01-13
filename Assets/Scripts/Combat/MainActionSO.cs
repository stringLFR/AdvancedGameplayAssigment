using UnityEngine;

[CreateAssetMenu(fileName = "MainActionSO", menuName = "Scriptable Objects/MainActionSO")]
public class MainActionSO : ScriptableObject
{
    [SerializeField]
    public MainActionStats mainActionStats;
}
