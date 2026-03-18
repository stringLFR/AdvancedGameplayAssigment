using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class AIController : ControllerBase
{
    private Combat monoCombat;
    private bool isdone = false;

    private int overdrives = 0;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetDoneBool(DroneUnitBody user)
    {
        if (user.Overdrive > overdrives)
        {

            CombatListener.AddLineToCombatText(user.DroneUnit.DroneName + $" has overdrive {user.Overdrive}! " +
                $"They can take {user.Overdrive - overdrives} more main actions!");

            overdrives++;

            if (UnityEngine.Random.Range(0f, 1f) > 1f - overdrives / user.Overdrive) user.SanityDamage(user.Overdrive);

            return;
        }

        isdone = true;

        overdrives = 0;
    }
}
