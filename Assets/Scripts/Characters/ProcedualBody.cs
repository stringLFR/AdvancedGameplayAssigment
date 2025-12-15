using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;



 namespace procedual
 {

    [Serializable]
    public struct bone
    {
        public Transform tr;
        public string name;
    }

    public enum procedualBodyType
    {
        NONE, ARM, LEG,
    }
    public sealed class ProcedualBody : MonoBehaviour
    {
        [SerializeField]
        procedualBodyType bodyType = procedualBodyType.NONE;

        public Transform root;

        [SerializeField]
        private bone[] bones;

        [SerializeField]
        public Vector3[] m_boneOffsets = new Vector3[3];

        [Header("Pole modifer")]
        [SerializeField]
        float rightVectorModifier = 3.0f, toParentModifier = 1.5f, downVectorModifier = 1.5f, toTargetModifier = 0.01f;


        [SerializeField]
        private bool isActive = false, debug = false;

        [SerializeField]
        private Transform debugTarget = null;

        private Vector3 pole;
        private Vector3 target;

        public Vector3 Target {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return target; } set { target = value; } }

        public bone[] Bones {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return bones; } }

        public procedualBodyType BodyType {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return bodyType; } }

        private Transform parent {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
            get { return transform.parent; } }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Target = bones.LastOrDefault().tr.position;
        }

        private void OnValidate()
        {
            List<bone> list = new List<bone>();

            SetupBones(list, this.transform);

            bones = list.ToArray();

        }

        // Update is called once per frame
        void Update()
        {
            if (debug == true && debugTarget != null) target = debugTarget.position;

            UpdateIK();
        }

        public void UpdateIK()
        {
            if (isActive == false) return;

            Vector3 toParent = (bones[0].tr.position - parent.position).normalized;

            switch (bodyType)
            {
                case procedualBodyType.NONE:

                    break;

                case procedualBodyType.ARM:
                    pole = parent.position + -parent.right * rightVectorModifier + toParent * toParentModifier + Vector3.down * downVectorModifier/* + -target * toTargetModifier*/;
                    //pole = root.TransformPoint(root.right);
                    break;
                case procedualBodyType.LEG:
                    pole = parent.position + parent.right * rightVectorModifier + toParent * toParentModifier + Vector3.down * downVectorModifier/* + target * toTargetModifier*/;
                    //pole = root.TransformPoint(-root.right);
                    break;
            }

            TwoBoneIk(bones, m_boneOffsets, target, pole);
        }

        private void TwoBoneIk(bone[] arr, Vector3[] boneAngleOffsets, Vector3 vTarget, Vector3 vPole)
        {
            if (arr == null || boneAngleOffsets == null) return;

            //Matrix4x4 test = Matrix4x4.Rotate(root.rotation);
            //test = test.transpose;

            Vector3 vTowardPole = vPole - bones[0].tr.position;
            Vector3 vTowardTarget = vTarget - bones[0].tr.position;

            //vTowardPole = math.mul((float3x3)(float4x4)test, new float3((float3)vTowardPole)).xyz;
            //vTowardTarget = math.mul((float3x3)(float4x4)test, new float3((float3)vTowardTarget)).xyz;

            float fRootBoneLength = Vector3.Distance(bones[0].tr.position, bones[2].tr.position);
            float fSecondBoneLength = Vector3.Distance(bones[2].tr.position, bones[4].tr.position);
            float fTotalChainLength = fRootBoneLength + fSecondBoneLength;

            // Align root with target
            bones[0].tr.rotation = Quaternion.LookRotation(vTowardTarget, vTowardPole);
            bones[0].tr.localRotation *= Quaternion.Euler(boneAngleOffsets[0]);

            Vector3 vTowardSecondBone = bones[2].tr.position - bones[0].tr.position;

            float fTargetDistance = Vector3.Distance(bones[0].tr.position, vTarget);

            // Limit hypotenuse to under the total bone distance to prevent invalid triangles
            fTargetDistance = Mathf.Min(fTargetDistance, fTotalChainLength * 0.9999f);

            // Solve for the angle for the root bone
            // See https://en.wikipedia.org/wiki/Law_of_cosines
            float fAdjacent = ((fRootBoneLength * fRootBoneLength) + (fTargetDistance * fTargetDistance) -
                                (fSecondBoneLength * fSecondBoneLength)) /
                                (2 * fTargetDistance * fRootBoneLength);

            float fAngle = Mathf.Acos(fAdjacent) * Mathf.Rad2Deg;

            // We rotate around the vector orthogonal to both pole and second bone
            Vector3 vCross = Vector3.Cross(vTowardPole, vTowardSecondBone);

            if (!float.IsNaN(fAngle)) bones[0].tr.RotateAround(bones[0].tr.position, vCross, -fAngle);

            // We've rotated the root bone to the right place, so we just 
            // look at the target from the elbow to get the final rotation
            Quaternion qSecondBoneTargetRotation = Quaternion.LookRotation(vTarget - bones[2].tr.position, vCross);
            qSecondBoneTargetRotation *= Quaternion.Euler(boneAngleOffsets[1]);
            bones[2].tr.rotation = qSecondBoneTargetRotation;
        }
        
        private void SetupBones(List<bone> list, Transform tr)
        {
            list.Add(new bone() { tr = tr, name = tr.name });

            foreach (Transform t in tr) SetupBones(list, t);
        }

        private void OnDrawGizmos()
        {
            Vector3 posA = transform.position;

            Gizmos.color = Color.red;

            Gizmos.DrawSphere(target, 0.075f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pole, 0.05f);
            for (int i = 1; i < bones.Length; i++)
            {
                Gizmos.DrawLine(posA, bones[i].tr.position);
                Gizmos.DrawLine(pole, bones[i].tr.position);
                posA = bones[i].tr.position;
                Gizmos.DrawSphere(posA, 0.05f);
            }
        }
    }
}

