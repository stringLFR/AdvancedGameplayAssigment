using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class ICombatObject_Step : ICombatObject
{
    protected DroneUnitBody myCaster;
    protected ActionEffectBase myOrigin;
    protected bool isActive = false;
    protected NavMeshPath path = new NavMeshPath();
    protected int points;
    protected int pointIndex = 0;
    protected float lerpTime;
    protected Vector3 directionVelocity;
    protected float moveSpeed = 100;

    public event Action<ICombatObject> MyActionDelegate;

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void CombatUpdate()
    {
        LerpAlongPath();
    }

    protected virtual void LerpAlongPath()
    {
        if (pointIndex >= points)
        {
            isActive = false;
            return;
        }

        if (Vector3.Distance(myCaster.transform.position, path.corners[pointIndex] + (myCaster.ProcedualCore.Agent.baseOffset * Vector3.up)) <= myCaster.ProcedualCore.Agent.radius)
        {
            pointIndex++;
            lerpTime = 0;

            if (pointIndex >= points)
            {
                isActive = false;
                return;
            }
        }

        Vector3 movementVecotr = (path.corners[pointIndex] + (myCaster.ProcedualCore.Agent.baseOffset * Vector3.up) - myCaster.transform.position).normalized;

        directionVelocity = Vector3.Lerp(directionVelocity, movementVecotr, Mathf.Clamp01(lerpTime * moveSpeed));

        myCaster.ProcedualCore.Agent.Move(directionVelocity * myCaster.ProcedualCore.Agent.speed * Time.deltaTime);

        myCaster.ProcedualCore.ManualNavRotTarget = path.corners[pointIndex];

        lerpTime += Time.deltaTime;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
    }

    public virtual void Reactivate(float mana, Vector3 targetPos)
    {
        isActive = true;

        path.ClearCorners();

        NavMesh.SamplePosition(targetPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

        NavMesh.CalculatePath(myCaster.transform.position, hit.position, NavMesh.AllAreas, path);

        points = path.corners.Length;
        pointIndex = 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public virtual void Reactivate(float mana)
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue()
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue(Vector3 triggerPos)
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue(DroneUnitBody triggeredDrone)
    {
        throw new System.NotImplementedException();
    }

    public bool FinalEffectReturnValue(GameObject triggeredObject)
    {
        throw new System.NotImplementedException();
    }

    public void MyRespondAction(ICombatObject obj)
    {
        throw new NotImplementedException();
    }
}
