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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
