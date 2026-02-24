using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;
    [SerializeField]
    Vector3 pa_Tangent, pb_Tangent, pa_Tangent_Rand, pb_Tangent_Rand;
    [SerializeField, Range(0,1)]
    private float speed = 1f;

    private ICombatObject controller;

    private Vector3 p0, p1, p2, p3;
    private float startingMana;
    private float progress;
    private bool hasHit = false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public void InitProjectile(ICombatObject c)
    {
        controller = c;
    }
    public bool moveProjectile()
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * speed;
        float i = Mathf.InverseLerp(0,1, UniGameMaths.EasingFunctionMaths.EaseInSine(progress));
        transform.position = GetCubicBezierPosition(i);

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Projectile ran out of mana!");
            return false;
        }
        if (Vector3.Distance(transform.position, p3) < 1)
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
        p0 = pa;
        p1 = pa + (pa_Tangent + RandomTangent(pa_Tangent_Rand));
        p2 = pb - (pb_Tangent + RandomTangent(pb_Tangent_Rand)); 
        p3 = pb;
        startingMana = mana;
        progress = 0f;
        hasHit = false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    private Vector3 RandomTangent(Vector3 value) => new Vector3(Random.Range(-value.x, value.x), Random.Range(-value.y, value.y), Random.Range(-value.z, value.z));

    private Vector3 GetCubicBezierPosition(float f)
    {
        float fOneMinusT = 1.0f - f;
        return p0 * fOneMinusT * fOneMinusT * fOneMinusT +
               p1 * 3 * fOneMinusT * fOneMinusT * f +
               p2 * 3 * fOneMinusT * f * f +
               p3 * f * f * f;
    }

    //Clamps given float!
    //Still needs some work! TODO!!!
    [MethodImpl(MethodImplOptions.AggressiveInlining)] //This is inline hint for jit compiler!
    public Vector3 GetPredictedPosition(float time, out float progressLeft)
    {
        progressLeft = progress;

        return GetCubicBezierPosition(Mathf.Clamp01(time));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            if (hit != controller.Caster)
            {
                hasHit = true;

                CombatListener.AddLineToCombatText($"Projectile hit {hit.DroneUnit.DroneName} with {(int)startingMana} mana left!");

                hit.TakeDamage(controller.Caster.MyRanged_P_HitRate, startingMana);
            }
        }
    }
}
