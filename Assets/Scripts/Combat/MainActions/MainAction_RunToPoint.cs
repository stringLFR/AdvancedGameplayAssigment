using actions;
using System.Collections;
using UnityEngine;

public class MainAction_RunToPoint : MainActionBase
{
    private string mainActionName;
    private string mainActionInfo;
    private ActionType actionType;
    private int manaCost;
    private effectType effectType;
    private ActionEffectBase effect;

    public override string MainActionName => mainActionName;

    public override ActionType ActionType => actionType;

    public override int ManaCost => manaCost;

    public override effectType EffectType => effectType;

    public override string MainActionInfo => mainActionInfo;

    public MainAction_RunToPoint(ActionEffectBase ae, string n, string i, int m, ActionType a, effectType e)
    {
        effect = ae;
        mainActionName = n;
        mainActionInfo = i;
        actionType = a;
        manaCost = m;
        effectType = e;
    }

    public override void Activate(ControllerBase controller, DroneUnitBody user)
    {

        if (controller is PlayerController)
        {
            user.StopAllCoroutines();
            user.StartCoroutine(PLayerInput(controller as PlayerController, user));
        }
        else if (controller is AIController)
        {

        }
    }

    IEnumerator PLayerInput(PlayerController p, DroneUnitBody d)
    {
        while (true)
        {

            if (MousePoint.instance.IsOverUI == true)
            {
                yield return null;
                continue;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (effect is AE_Move)
                {
                    effect.TriggerActionEffect(d, MousePoint.instance.transform.position);

                    CombatListener.AddLineToCombatText(d.DroneUnit.DroneName + " Used MainAction_RunToPoint!");

                    ActivateADS(this);

                    break;
                }
            }


            yield return null;
        }

        p.SetDoneBool();

        yield return null;
    }
}
