using UnityEngine;

public class AE_Move : ActionEffectBase
{
    public AE_Move()
    {
        step = new ICombatObject_Step();
    }

    private ICombatObject_Step step;
    public override void TriggerActionEffect(DroneUnitBody caster)
    {
        Init();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, Vector3 targetPos)
    {
        Init();

        step.OnSpawn(caster, this);//Might allow things to take over. So leaving this function for now!

        step.Reactivate(caster.MyMana, targetPos);
    }

    public override void TriggerActionEffect(DroneUnitBody caster, Vector3[] targetPositions)
    {
        Init();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, DroneUnitBody otherCaster)
    {
        Init();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, DroneUnitBody[] otherCasters)
    {
        Init();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, GameObject targetObj)
    {
        Init();
    }

    public override void TriggerActionEffect(DroneUnitBody caster, GameObject[] targetObjs)
    {
        Init();
    }

    private void Init()
    {
        if (step.IsActive == true) return;

        Combat.actionEffectObjects.Add(step);
    }
}
