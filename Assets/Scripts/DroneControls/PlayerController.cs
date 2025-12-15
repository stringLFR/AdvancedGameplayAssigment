using UnityEngine;
using UnityEngine.UI;

public class PlayerController : ControllerBase
{
    private Combat monoCombat;
    private bool done = false;
    public override bool isDone => done;

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

        if (user.MainActions.Count >= 1)
        {
            foreach (var a in user.MainActions)
            {
                if (index > playerHudPrototype.buttons.Count - 1) break;

                playerHudPrototype.buttons[index].onClick.AddListener(() => { a.Activate(this, user); });
                index++;
            }
        }
        else done = true;
    }

    public void SetDoneBool()
    {
        done = true;
    }

    public PlayerController(Combat c)
    {
        monoCombat = c;
    }
}
