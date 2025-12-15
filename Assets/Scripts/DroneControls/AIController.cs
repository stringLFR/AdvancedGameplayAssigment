using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AIController : ControllerBase
{
    private Combat monoCombat;
    private bool isdone = false;
    public override bool isDone => isdone;

    public override void ControllerDisable(DroneUnitBody user)
    {
        Debug.Log("AI TURN END");
        isdone = false;
    }

    public override void ControllerEnable(DroneUnitBody user)
    {
        Debug.Log("AI TURN START");
        monoCombat.StartCoroutine(CalculateAction(user));
    }

    private IEnumerator CalculateAction(DroneUnitBody user)
    {
        Debug.Log("corutine Start");
        yield return new WaitForSeconds(1);

        isdone = true;
        Debug.Log("corutine end");
    }

    public AIController(Combat c)
    {
        monoCombat = c;
    }
}
