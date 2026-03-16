using System;
using UniGameMaths;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private ICombatObject controller;

    public DroneUnitBody test;
    public bool isTesting = false;


    private float startingMana;
    private float progress;

    [Serializable]
    public struct SlashData
    {
        public BezierCurvesMaths.CubicBezierCurve curve;
        public Vector3 BezierStartOffset, BezierEndOffset, BezierStartTangent, BezierEndOTanget;
        public AnimationCurve Xsize, Ysize, Zsize;
        public DroneUnitBody user;

        public SlashData(
            Vector3 startOffset, 
            Vector3 endOffset, 
            Vector3 startTangent, 
            Vector3 endTangent,
            AnimationCurve xSize,
            AnimationCurve ySize,
            AnimationCurve zSize,
            DroneUnitBody drone)
        {
            curve = new BezierCurvesMaths.CubicBezierCurve();
            BezierStartOffset = startOffset;
            BezierEndOffset = endOffset;
            BezierStartTangent = startTangent;
            BezierEndOTanget = endTangent;
            Xsize = xSize;
            Ysize = ySize;
            Zsize = zSize;
            user = drone;
        }

        public void SlashUpdate(float time, Transform tr)
        {
            Mathf.Clamp01(time);

            float X = 1 * Xsize.Evaluate(time);
            float Y = 1 * Ysize.Evaluate(time);
            float Z = 1 * Zsize.Evaluate(time);

            tr.localScale = new Vector3(X, Y, Z);

            tr.position = UnityMaths.GetUnityVecFromNumericsVec(curve.GetCubicBezierPosition(time));

            //tr.rotation = Quaternion.LookRotation(user.transform.position - tr.position, Vector3.up);
        }

        public void SetupSlash(Transform tr, Vector3 startTangentRandom, Vector3 endTangentRandom)
        {
            Vector3 s = new Vector3(tr.localPosition.x + BezierStartOffset.x, tr.localPosition.y + BezierStartOffset.y, tr.localPosition.z + BezierStartOffset.z);
            Vector3 e = new Vector3(tr.localPosition.x + BezierEndOffset.x, tr.localPosition.y + BezierEndOffset.y, tr.localPosition.z + BezierEndOffset.z);

            curve.SetVectors(UnityMaths.GetNumericsVecFromUnityVec(s),
            UnityMaths.GetNumericsVecFromUnityVec(e),
            UnityMaths.GetNumericsVecFromUnityVec(BezierStartTangent + Projectile.RandomTangent(startTangentRandom)),
            UnityMaths.GetNumericsVecFromUnityVec(BezierEndOTanget + Projectile.RandomTangent(endTangentRandom)));
        }
    }

    public SlashData data;

    [SerializeField,Range(0f,1f)]
    public float time = 0;

    public void InitMelee(ICombatObject c)
    {
        controller = c;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTesting == false) return;

        data.SetupSlash(test.transform, Vector3.zero, Vector3.zero);
        data.SlashUpdate(time, transform);
    }

    public bool Swinging()
    {
        return false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            CombatListener.AddLineToCombatText($"Projectile hit {hit.DroneUnit.DroneName} with {(int)startingMana} mana left!");

            hit.TakeDamage(controller.Caster.MyRanged_P_HitRate, startingMana);
        }
    }
}
