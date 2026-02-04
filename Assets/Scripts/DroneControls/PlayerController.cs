using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : ControllerBase
{
    private Combat monoCombat;
    private bool done = false;
    public override bool isDone => done;

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

        foreach (Button b in playerHudPrototype.buttons) b.onClick.RemoveAllListeners();

        foreach (TextMeshProUGUI t in playerHudPrototype.texts) t.text = "Empty";

        if (user.MainActions.Count >= 1)
        {
            foreach (var a in user.MainActions)
            {
                if (index > playerHudPrototype.buttons.Count - 1) break;

                playerHudPrototype.buttons[index].onClick.AddListener(() => { a.Activate(this, user); });
                playerHudPrototype.texts[index].text = a.MainActionName;
                index++;
            }
        }
        else done = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetDoneBool()
    {
        done = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public PlayerController(Combat c)
    {
        monoCombat = c;
    }
}
