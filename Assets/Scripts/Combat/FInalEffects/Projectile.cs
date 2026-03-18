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
    private float progressSpeed = 1f;
    [SerializeField]
    private MeleeSynergy[] meleeSynergy;

    [SerializeField]
    private AddedEffectSO[] addedEffects;

    [SerializeField]
    private bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    [SerializeField]
    private bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }

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
        progress += Time.deltaTime * progressSpeed;
        float i = Mathf.InverseLerp(0,1, EasingFunctionMaths.EaseInSine(progress));
        transform.position = UnityMaths.GetUnityVecFromNumericsVec(curve.GetCubicBezierPosition(i));

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Projectile ran out of mana!");

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, this, null, null);
            }

            return false;
        }
        if (Vector3.Distance(transform.position, UnityMaths.GetUnityVecFromNumericsVec(curve.GetLastPoint())) < 1)
        {
            CombatListener.AddLineToCombatText($"Projectile missed!");

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, this, null, null);
            }

            return false;
        }
        if (hasHit == true)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, this, null, null);
            }

            return false;
        }

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnProgress(progress, null, this, null, null);
        }

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

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnStarted(null,this,null,null);
        }
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
                    Vector3 end = CombatListener.GetClosesTarget(attacker.IsEnemy, start).transform.position;

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
                    added.OnTargetFound(hit, null, this, null, null);
                }
            }

            if (canPierce == true) hasHit = false;

            return;
        }

        if (canPierce == true) return;

        hasHit = true;
    }
}
