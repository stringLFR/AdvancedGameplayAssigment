using actions;
using System.Collections;
using UnityEngine;

public class MainAction_RunToPoint : MainActionBase
{
    private string mainActionName;
    private ActionType actionType;
    private int manaCost;
    private effectType effectType;
    private ActionEffectBase effect;

    public override string MainActionName => mainActionName;

    public override ActionType ActionType => actionType;

    public override int ManaCost => manaCost;

    public override effectType EffectType => effectType;

    public MainAction_RunToPoint(ActionEffectBase ae, string n, int m, ActionType a, effectType e)
    {
        effect = ae;
        mainActionName = n;
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
