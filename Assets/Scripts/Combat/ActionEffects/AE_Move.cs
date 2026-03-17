using System.Runtime.CompilerServices;
using UnityEngine;

public class AE_Move : ActionEffectBase
{
    public AE_Move()
    {
        step = new ICombatObject_Step();
    }

    protected ICombatObject_Step step;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster)
    {
        Init();
    }

    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3 targetPos)
    {
        Init();

        step.OnSpawn(caster, this);//Might allow things to take over. So leaving this function for now!

        step.Reactivate(caster.MyMana, targetPos);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, Vector3[] targetPositions)
    {
        Init();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody otherCaster)
    {
        Init();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, DroneUnitBody[] otherCasters)
    {
        Init();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject targetObj)
    {
        Init();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public override void TriggerActionEffect(float mana, DroneUnitBody caster, GameObject[] targetObjs)
    {
        Init();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    protected void Init()
    {
        if (step.IsActive == true) return;

        Combat.actionEffectObjects.Add(step);
    }
}
