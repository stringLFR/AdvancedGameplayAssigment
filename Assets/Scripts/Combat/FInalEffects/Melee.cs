using System;
using System.Security.Cryptography;
using UniGameMaths;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Melee : MonoBehaviour
{
    private ICombatObject controller;

    public DroneUnitBody test;
    public bool isTesting = false;

    private float startingMana;
    private float progress;

    public float Progress { get; set; }

    [SerializeField]
    private AddedEffectSO[] addedEffects;

    [SerializeField]
    private bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    [SerializeField]
    private bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }

    [Serializable]
    public struct SlashData
    {
        public BezierCurvesMaths.CubicBezierCurve curve;
        public BezierCurvesMaths.CubicBezierCurve rotateCurve;
        public Vector3 BezierStartOffset, BezierEndOffset, BezierStartTangent, BezierEndOTanget;
        public Vector3 BezierStartRotate, BezierEndRotate, BezierStartTangentRotate, BezierEndOTangetRotate;
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
            DroneUnitBody drone,
            Vector3 startRotate,
            Vector3 endRotate,
            Vector3 startTangentRotate,
            Vector3 endTangentRotate)
        {
            curve = new BezierCurvesMaths.CubicBezierCurve();
            rotateCurve = new BezierCurvesMaths.CubicBezierCurve();
            BezierStartOffset = startOffset;
            BezierEndOffset = endOffset;
            BezierStartTangent = startTangent;
            BezierEndOTanget = endTangent;
            Xsize = xSize;
            Ysize = ySize;
            Zsize = zSize;
            user = drone;
            BezierStartRotate = startRotate;
            BezierEndRotate = endRotate;
            BezierStartTangentRotate = startTangentRotate;
            BezierEndOTangetRotate = endTangentRotate;
        }

        public void SlashUpdate(float time, Transform tr)
        {
            float X = 1 * Xsize.Evaluate(time);
            float Y = 1 * Ysize.Evaluate(time);
            float Z = 1 * Zsize.Evaluate(time);

            tr.localScale = new Vector3(X, Y, Z);

            tr.position = UnityMaths.GetUnityVecFromNumericsVec(curve.GetCubicBezierPosition(time));

            tr.up = UnityMaths.GetUnityVecFromNumericsVec(rotateCurve.GetCubicBezierPosition(time)) - tr.position;
        }

        public void SetupSlash(Transform tr, Vector3 startTangentRandom, Vector3 endTangentRandom)
        {
            Vector3 localPos = tr.InverseTransformPoint(tr.position);
            Vector3 s = localPos + BezierStartOffset;
            Vector3 e = localPos + BezierEndOffset;
            Vector3 sT = localPos + (BezierStartTangent + Projectile.RandomTangent(startTangentRandom));
            Vector3 eT = localPos + (BezierStartTangent + Projectile.RandomTangent(endTangentRandom));

            Vector3 rs = localPos + BezierStartRotate;
            Vector3 re = localPos + BezierEndRotate;
            Vector3 rsT = localPos + (BezierStartTangentRotate);
            Vector3 reT = localPos + (BezierEndOTangetRotate);


            curve.SetVectors(UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(s)),
            UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(e)),
            UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(sT)),
            UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(eT)));

            rotateCurve.SetVectors(UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(rs)),
            UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(re)),
            UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(rsT)),
            UnityMaths.GetNumericsVecFromUnityVec(tr.TransformPoint(reT)));
        }
    }

    public SlashData[] data;

    public int MaxSlashes => data.Length - 1;

    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;

    [SerializeField, Range(0, 1)]
    private float progressSpeed = 1f;

    [SerializeField,Range(0f,10f)]
    public float time = 0;

    [SerializeField,Range(0.05f,1f)]
    private float gizmosAccuracy = 0.1f;

    public void InitMelee(ICombatObject c)
    {
        controller = c;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTesting == false) return;

        int index = (int)Math.Clamp(time, 0f, data.Length -1);

        data[index].SetupSlash(test.transform, Vector3.zero, Vector3.zero);
        data[index].SlashUpdate(time % 1, transform);
    }

    public void UnSheath(float mana)
    {
        foreach (var t in data) t.SetupSlash(controller.Caster.transform, Vector3.zero, Vector3.zero);

        startingMana = mana;
        progress = 0f;

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnStarted(null,null,this,null);
        }
    }

    
    public bool Swinging()
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * progressSpeed;

        int index = (int)Math.Clamp(progress, 0f, data.Length - 1);

        data[index].SlashUpdate(time % 1, transform);

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Melee ran out of mana!");

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, null, this, null);
            }

            return false;
        }

        if ((int)progress > data.Length - 1)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, null, this, null);
            }

            return false;
        }

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnProgress(progress, null, null, this, null);
        }

        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            bool hasHit = controller.FinalEffectReturnValue(hit);

            if (hasHit == true)
            {
                CombatListener.AddLineToCombatText($"Melee hit {hit.DroneUnit.DroneName} with {(int)startingMana} mana left!");

                if (isDamaging == true)
                {
                    hit.TakeDamage((int)controller.myDamageType, startingMana);
                }
                else
                {
                    hit.Heal((int)controller.myDamageType, startingMana);
                }

                foreach (AddedEffectSO added in addedEffects)
                {
                    added.OnTargetFound(hit, null, null, this, null);
                }
            }
        }

        if (other.TryGetComponent<Projectile>(out Projectile projectile) == true)
        {
            projectile.OnHitByMelee(controller.Caster);

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnProjectileFound(projectile, null, null, this, null);
            }
        }

        if (other.TryGetComponent<Melee>(out Melee otherMelee) == true)
        {
            //Both strikes block eachother!
            otherMelee.Progress = (int)Math.Clamp(otherMelee.Progress, 0f, otherMelee.MaxSlashes) + 1;
            progressSpeed = (int)Math.Clamp(progressSpeed, 0f, data.Length - 1) + 1;

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnMeleeFound(otherMelee, null, null, this, null);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isTesting == false) return;

        int index = (int)Math.Clamp(time, 0f, data.Length - 1);

        for(float i = 0; i < 1f; i += gizmosAccuracy)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                UnityMaths.GetUnityVecFromNumericsVec(data[index].curve.GetCubicBezierPosition(i)), 
                UnityMaths.GetUnityVecFromNumericsVec(data[index].curve.GetCubicBezierPosition(i + gizmosAccuracy)));

            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                UnityMaths.GetUnityVecFromNumericsVec(data[index].rotateCurve.GetCubicBezierPosition(i)),
                UnityMaths.GetUnityVecFromNumericsVec(data[index].rotateCurve.GetCubicBezierPosition(i + gizmosAccuracy)));
        }

        Gizmos.color = Color.green;

        Gizmos.DrawLine(
                UnityMaths.GetUnityVecFromNumericsVec(data[index].curve.GetCubicBezierPosition(time % 1)),
                UnityMaths.GetUnityVecFromNumericsVec(data[index].rotateCurve.GetCubicBezierPosition(time % 1)));
    }
}
