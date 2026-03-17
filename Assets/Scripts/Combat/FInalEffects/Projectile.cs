using System.Runtime.CompilerServices;
using UniGameMaths;
using UnityEngine;

public enum MeleeSynergy
{
    NONE,DEFLECT, BLOCK, REDIRECT,
}

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;
    [SerializeField]
    Vector3 pa_Tangent, pb_Tangent, pa_Tangent_Rand, pb_Tangent_Rand;
    [SerializeField, Range(0,1)]
    private float speed = 1f;
    [SerializeField]
    private MeleeSynergy[] meleeSynergy;

    private ICombatObject controller;
    private BezierCurvesMaths.CubicBezierCurve curve = new BezierCurvesMaths.CubicBezierCurve();
    private float startingMana;
    private float progress;
    private bool hasHit = false;

    [SerializeField]
    private bool canPierce = false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void InitProjectile(ICombatObject c)
    {
        controller = c;
    }
    public bool moveProjectile()
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * speed;
        float i = Mathf.InverseLerp(0,1, EasingFunctionMaths.EaseInSine(progress));
        transform.position = UnityMaths.GetUnityVecFromNumericsVec(curve.GetCubicBezierPosition(i));

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Projectile ran out of mana!");
            return false;
        }
        if (Vector3.Distance(transform.position, UnityMaths.GetUnityVecFromNumericsVec(curve.GetLastPoint())) < 1)
        {
            CombatListener.AddLineToCombatText($"Projectile missed!");
            return false;
        }
        if (hasHit == true) return false;

        return true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void Fire(float mana, Vector3 pa, Vector3 pb)
    {
        curve.SetVectors(UnityMaths.GetNumericsVecFromUnityVec(pa), 
            UnityMaths.GetNumericsVecFromUnityVec(pb), 
            UnityMaths.GetNumericsVecFromUnityVec(pa_Tangent + RandomTangent(pa_Tangent_Rand)), 
            UnityMaths.GetNumericsVecFromUnityVec(pb_Tangent + RandomTangent(pb_Tangent_Rand)));
        startingMana = mana;
        progress = 0f;
        hasHit = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public static Vector3 RandomTangent(Vector3 value) => new Vector3(UnityEngine.Random.Range(-value.x, value.x), UnityEngine.Random.Range(-value.y, value.y), UnityEngine.Random.Range(-value.z, value.z));

    //Clamps given float!
    //Still needs some work! TODO!!!
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public Vector3 GetPredictedPosition(float time, out float progressLeft)
    {
        progressLeft = progress;

        return UnityMaths.GetUnityVecFromNumericsVec(curve.GetCubicBezierPosition(Mathf.Clamp01(time)));
    }

    public void OnHitByMelee(DroneUnitBody attacker)
    {
        foreach(MeleeSynergy m in meleeSynergy)
        {
            switch (m)
            {
                case MeleeSynergy.NONE:
                    break;
                case MeleeSynergy.DEFLECT:

                    Vector3 pa = UnityMaths.GetUnityVecFromNumericsVec(curve.GetFirstPoint());
                    Vector3 pb = UnityMaths.GetUnityVecFromNumericsVec(curve.GetLastPoint());

                    curve.SetVectors(UnityMaths.GetNumericsVecFromUnityVec(pb),
                        UnityMaths.GetNumericsVecFromUnityVec(pa),
                        UnityMaths.GetNumericsVecFromUnityVec(pa_Tangent + RandomTangent(pa_Tangent_Rand)),
                        UnityMaths.GetNumericsVecFromUnityVec(pb_Tangent + RandomTangent(pb_Tangent_Rand)));

                    progress = 0f;

                    break;
                case MeleeSynergy.BLOCK:

                    hasHit = true;

                    break;
                case MeleeSynergy.REDIRECT:

                    Vector3 start = transform.position;
                    Vector3 end = CombatListener.GetClosesTarget(CombatListener.IsEnemy(attacker), start).transform.position;

                    curve.SetVectors(UnityMaths.GetNumericsVecFromUnityVec(start),
                        UnityMaths.GetNumericsVecFromUnityVec(end),
                        UnityMaths.GetNumericsVecFromUnityVec(pa_Tangent + RandomTangent(pa_Tangent_Rand)),
                        UnityMaths.GetNumericsVecFromUnityVec(pb_Tangent + RandomTangent(pb_Tangent_Rand)));

                    progress = 0f;

                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            hasHit = controller.FinalEffectReturnValue(hit);

            if (hasHit == true)
            {
                CombatListener.AddLineToCombatText($"Projectile hit {hit.DroneUnit.DroneName} with {(int)startingMana} mana left!");

                hit.TakeDamage(controller.Caster.MyRanged_P_HitRate, startingMana);
            }

            if (canPierce == true) hasHit = false;

            return;
        }

        if (canPierce == true) return;

        hasHit = true;
    }
}
