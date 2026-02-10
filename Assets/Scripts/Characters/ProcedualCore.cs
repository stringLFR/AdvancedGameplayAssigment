using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public sealed class ProcedualCore : MonoBehaviour
{
    [Header("Navmesh")]
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Animator animator;

    public Animator Animator => animator;
    private Vector3 manualNavRotTarget = Vector3.zero;

    [Header("ProcedualBodies")]
    [SerializeField]
    private procedual.ProcedualBody leftArm, leftLeg, rightArm,rightLeg;

    [Header("Bones")]
    [SerializeField]
    private procedual.bone core, root, body, hip, shoulders, neck, head;

    public procedual.bone Root => root;

    [Header("TargetPoint")]
    [SerializeField]
    Vector3 r_P_arm_default, l_P_arm_default, r_P_leg_default, l_P_leg_default;

    Vector3 r_P_arm_added, l_P_arm_added, r_P_leg_added, l_P_leg_added;

    Vector3 r_P_arm_current, l_P_arm_current, r_P_leg_current, l_P_leg_current;

    [Header("Lerps")]
    [SerializeField]
    float targetPointLerpSpeed = 0.5f, agentRotationLerpSpeed = 0.5f;

    [SerializeField]
    LayerMask raycastIgnore;

    public NavMeshAgent Agent => agent;

    public Vector3 ManualNavRotTarget {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        get { return manualNavRotTarget; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
        set { manualNavRotTarget = value; } }

    private Vector3 rpa => r_P_arm_default + r_P_arm_added;
    private Vector3 lpa => l_P_arm_default + l_P_arm_added; 
    private Vector3 rpl => r_P_leg_default + r_P_leg_added;
    private Vector3 lpl => l_P_leg_default + l_P_leg_added; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        leftArm.root = root.tr;
        leftLeg.root = root.tr;
        rightArm.root = root.tr;
        rightLeg.root = root.tr;
    }

    // Update is called once per frame
    void Update()
    {

        NavManualRotation();
        MatrixRotation();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetNewNavTarget(Vector3 pos) => agent.destination = pos;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void SetNewManualNavRotationTarget(Vector3 pos) => manualNavRotTarget = pos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    private void NavManualRotation()
    {
        Quaternion lookDir = Quaternion.LookRotation(manualNavRotTarget - transform.position, transform.up);
        Vector3 vecDit = lookDir.eulerAngles;
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, vecDit.y - 90, transform.eulerAngles.z), Time.deltaTime * agentRotationLerpSpeed);
    }

    private Vector3 RaycastToTarget(Vector3 target, Vector3 start, Matrix4x4 rotMatrix, Transform limbEnd = null)
    {
        Vector3 rot = rotMatrix.MultiplyPoint3x4(target) + transform.localPosition;
        Vector3 dir = rot - start ;

        if (Physics.SphereCast(start,0.2f, dir, out RaycastHit hit, dir.magnitude, ~raycastIgnore) == true)
        {
            if (limbEnd != null)
            {
                limbEnd.rotation = limbEnd.parent.rotation;

                Vector3 rotation = Vector3.Cross(limbEnd.right, hit.normal);

                limbEnd.rotation = Quaternion.LookRotation(rotation, hit.normal);
            }

            return hit.point;
        }
        else
        {
            if (limbEnd != null) limbEnd.rotation = limbEnd.parent.rotation;

            return rot;
        }
    }

    private void MatrixRotation()
    {
        // Doing this to make sure rotation works when taking from local to world!
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(transform.rotation);

        // Rotate before translting!
        r_P_arm_current = RaycastToTarget(rpa, rightArm.Bones[0].tr.position, rotationMatrix);
        l_P_arm_current = RaycastToTarget(lpa, leftArm.Bones[0].tr.position, rotationMatrix);
        r_P_leg_current = RaycastToTarget(rpl, rightLeg.Bones[0].tr.position, rotationMatrix, rightLeg.Bones[4].tr);
        l_P_leg_current = RaycastToTarget(lpl, leftLeg.Bones[0].tr.position, rotationMatrix, leftLeg.Bones[4].tr);

        float lerpValue = Time.deltaTime * targetPointLerpSpeed;

        leftArm.Target = Vector3.LerpUnclamped(leftArm.Target, l_P_arm_current, lerpValue);
        leftLeg.Target = Vector3.LerpUnclamped(leftLeg.Target, l_P_leg_current, lerpValue);
        rightArm.Target = Vector3.LerpUnclamped(rightArm.Target, r_P_arm_current, lerpValue);
        rightLeg.Target = Vector3.LerpUnclamped(rightLeg.Target, r_P_leg_current, lerpValue);
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(root.tr.position, 0.05f);
        Gizmos.DrawSphere(body.tr.position, 0.05f);
        Gizmos.DrawSphere(hip.tr.position, 0.05f);
        Gizmos.DrawSphere(shoulders.tr.position, 0.05f);
        Gizmos.DrawSphere(head.tr.position, 0.05f);
        Gizmos.DrawSphere(neck.tr.position, 0.05f);

        Gizmos.DrawLine(body.tr.position, hip.tr.position);
        Gizmos.DrawLine(body.tr.position, shoulders.tr.position);

        Gizmos.DrawLine(shoulders.tr.position, neck.tr.position);
        Gizmos.DrawLine(shoulders.tr.position, rightArm.transform.position);
        Gizmos.DrawLine(shoulders.tr.position, leftArm.transform.position);

        Gizmos.DrawLine(hip.tr.position, rightLeg.transform.position);
        Gizmos.DrawLine(hip.tr.position, leftLeg.transform.position);

        Gizmos.DrawLine(neck.tr.position, head.tr.position);
        Gizmos.DrawLine(root.tr.position, hip.tr.position);
        Gizmos.DrawLine(root.tr.position, core.tr.position);
    }
}
