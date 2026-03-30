using actions;
using System;
using System.Collections;
using UnityEngine;


public class MainAction_TargetPoint : MainActionBase
{
    private string mainActionName;
    private string mainActionInfo;
    private ActionType actionType;
    private int manaCost;
    private effectType effectType;
    private ActionEffectBase effect;


    public void SetTargetPointPrefabPath(String PrefabPath)
    {
        targetPointPrefab = PrefabPath;

        effect.SetAssetPath(targetPointPrefab);
    }

    private string targetPointPrefab;

    public override string MainActionName => mainActionName;

    public override ActionType ActionType => actionType;

    public override int ManaCost => manaCost;

    public override effectType EffectType => effectType;

    public override string MainActionInfo => mainActionInfo;

    public MainAction_TargetPoint(ActionEffectBase ae, string n, string i, int m, ActionType a, effectType e)
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
                d.ManaSpent(manaCost);

                if (d.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{d.DroneUnit.DroneName} Ran out of mana!");
                    break;
                }

                effect.TriggerActionEffect(manaCost, d, MousePoint.instance.transform.position);

                CombatListener.AddLineToCombatText(d.DroneUnit.DroneName + $" Used MainAction {mainActionName}!");

                ActivateADS(this);

                break;
            }


            yield return null;
        }

        p.SetDoneBool(d);

        yield return null;
    }
}

public class MainAction_TargetManyPoints : MainActionBase
{
    private string mainActionName;
    private string mainActionInfo;
    private ActionType actionType;
    private int manaCost;
    private effectType effectType;
    private ActionEffectBase effect;


    public void SetTargetManyPrefabPath(String PrefabPath)
    {
        targetPointPrefab = PrefabPath;

        effect.SetAssetPath(targetPointPrefab);
    }

    public void Init(float radius)
    {
        targetRadius = radius;
    }

    private float targetRadius;

    private string targetPointPrefab;

    public override string MainActionName => mainActionName;

    public override ActionType ActionType => actionType;

    public override int ManaCost => manaCost;

    public override effectType EffectType => effectType;

    public override string MainActionInfo => mainActionInfo;

    public MainAction_TargetManyPoints(ActionEffectBase ae, string n, string i, int m, ActionType a, effectType e)
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
                d.ManaSpent(manaCost);

                if (d.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{d.DroneUnit.DroneName} Ran out of mana!");
                    break;
                }

                Collider[] colliders = Physics.OverlapSphere(MousePoint.instance.transform.position, targetRadius,1<<6);

                Vector3[] positions = new Vector3[colliders.Length];

                for (int i = 0; i < positions.Length; i++) positions[i] = colliders[i].transform.position;

                effect.TriggerActionEffect(manaCost, d, positions);

                CombatListener.AddLineToCombatText(d.DroneUnit.DroneName + $" Used MainAction {mainActionName}!");

                ActivateADS(this);

                break;
            }


            yield return null;
        }

        p.SetDoneBool(d);

        yield return null;
    }
}

public class MainAction_TargetSelf : MainActionBase
{
    private string mainActionName;
    private string mainActionInfo;
    private ActionType actionType;
    private int manaCost;
    private effectType effectType;
    private ActionEffectBase effect;


    public void SetTargetSelfPrefabPath(String PrefabPath)
    {
        targetPointPrefab = PrefabPath;

        effect.SetAssetPath(targetPointPrefab);
    }

    private string targetPointPrefab;

    public override string MainActionName => mainActionName;

    public override ActionType ActionType => actionType;

    public override int ManaCost => manaCost;

    public override effectType EffectType => effectType;

    public override string MainActionInfo => mainActionInfo;

    public MainAction_TargetSelf(ActionEffectBase ae, string n, string i, int m, ActionType a, effectType e)
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
                d.ManaSpent(manaCost);

                if (d.MyMana < 0)
                {
                    CombatListener.AddLineToCombatText($"{d.DroneUnit.DroneName} Ran out of mana!");
                    break;
                }

                effect.TriggerActionEffect(manaCost, d, d.transform.position);

                CombatListener.AddLineToCombatText(d.DroneUnit.DroneName + $" Used MainAction {mainActionName}!");

                ActivateADS(this);

                break;
            }


            yield return null;
        }

        p.SetDoneBool(d);

        yield return null;
    }
}
