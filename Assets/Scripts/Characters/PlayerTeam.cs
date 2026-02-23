using System;
using UnityEngine;


public class PlayerTeam : MonoBehaviour
{
    [SerializeField]
    private Squad playerMembers;

    public Squad PlayerMembers => playerMembers;

    public int PlayerTeamAverageLevel
    {
        get
        {
            float totalLevels = 0;
            float totalMembers = 0;

            foreach (var member in playerMembers.droneUnits)
            {
                totalLevels += member.Level;
                totalMembers++;
            }

            return (int)(totalLevels / totalMembers);
        }
    }
}
