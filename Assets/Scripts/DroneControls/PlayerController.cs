using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerController : ControllerBase
{
    private Combat monoCombat;
    private bool done = false;
    public override bool isDone => done;

    private int overdrives = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void ControllerDisable(DroneUnitBody user)
    {
        Debug.Log("PLAYER TURN END");

        monoCombat.PlayerHUD.SetActive(false);

        done = false;
    }

    public override void ControllerEnable(DroneUnitBody user)
    {
        Debug.Log("PLAYER TURN START");
        monoCombat.PlayerHUD.SetActive(true);

        PlayerHudPrototype playerHudPrototype = monoCombat.PlayerHUD.GetComponent<PlayerHudPrototype>();

        int index = 0;

        foreach (TextMeshProUGUI t in playerHudPrototype.texts) t.text = "Empty";

        foreach (Button b in playerHudPrototype.buttons)
        {
            b.onClick.RemoveAllListeners();
            b.gameObject.SetActive(false);
        }

        if (user.MainActions.Count >= 1)
        {
            foreach (var a in user.MainActions)
            {
                if (index > playerHudPrototype.buttons.Count - 1) break;

                playerHudPrototype.buttons[index].gameObject.SetActive(true);
                playerHudPrototype.buttons[index].onClick.AddListener(() => { a.Activate(this, user); });
                playerHudPrototype.texts[index].text = a.MainActionName;
                playerHudPrototype.hoverTriggers[index].SetInfo(a.MainActionName,a.MainActionInfo);
                index++;
            }
        }
        else done = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetDoneBool(DroneUnitBody user)
    {
        if (user.Overdrive > overdrives)
        {
            CombatListener.AddLineToCombatText(user.DroneUnit.DroneName + $" has overdrive {user.Overdrive}! " +
                $"They can take {user.Overdrive - overdrives} more main actions!");

            overdrives++;

            if (UnityEngine.Random.Range(0f, 1f) > 1f - overdrives/user.Overdrive) user.SanityDamage(user.Overdrive);

            return;
        }

        done = true;
        MousePoint.instance.Projector.size = Vector3.one;
        overdrives = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public PlayerController(Combat c)
    {
        monoCombat = c;
    }
}
