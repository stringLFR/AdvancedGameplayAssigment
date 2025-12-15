using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class ICombatObject_Step : ICombatObject
{
    private DroneUnitBody myCaster;
    private ActionEffectBase myOrigin;
    private bool isActive = false;
    private NavMeshPath path = new NavMeshPath();
    private int points;
    private int pointIndex = 0;
    private float lerpTime;
    private Vector3 directionVelocity;
    private float moveSpeed = 100;

    public bool IsActive => isActive;

    public DroneUnitBody Caster => myCaster;

    public ActionEffectBase Origin => myOrigin;

    public void CombatUpdate()
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

    public void OnSpawn(DroneUnitBody caster, ActionEffectBase origin)
    {
        myCaster = caster;
        myOrigin = origin;
    }

    public void Reactivate(float mana, Vector3 targetPos)
    {
        isActive = true;

        path.ClearCorners();

        NavMesh.SamplePosition(targetPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

        NavMesh.CalculatePath(myCaster.transform.position, hit.position, NavMesh.AllAreas, path);

        points = path.corners.Length;
        pointIndex = 0;
    }

    public void Reactivate(float mana, DroneUnitBody otherCaster)
    {
        throw new System.NotImplementedException();
    }

    public void Reactivate(float mana, GameObject targetObj)
    {
        throw new System.NotImplementedException();
    }

    public void Reactivate(float mana)
    {
        throw new System.NotImplementedException();
    }
}
