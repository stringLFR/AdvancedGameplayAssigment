using UniGameMaths;
using UnityEngine;
using UnityEngine.AI;

public class SummonObject : MonoBehaviour
{
    private ICombatObject controller;
    private float startingMana;
    private float progress;

    [SerializeField]
    private NavMeshObstacle navMeshObstacle;

    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;

    [SerializeField, Range(0, 1)]
    private float progressSpeed = 1f;

    [SerializeField]
    private AddedEffectSO[] addedEffects;

    [SerializeField]
    private float lifetime;

    [SerializeField]
    private bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    [SerializeField]
    private bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }

    public void InitSummonedObject(ICombatObject c)
    {
        controller = c;
    }

    public void Summon(float mana, Vector3 pos, Vector3 lookPos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(lookPos, Vector3.up);
        startingMana = mana;
        progress = 0f;

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnStarted(this);
        }
    }

    public bool SustainSummon(Vector3 pos)
    {
        if (controller.Caster.AppliedStatusDict.TryGetValue(Status_Stunned.StunnedKey, out StatusBase status) == true)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(this);
            }

            return false;
        }

        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * progressSpeed;

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Summon ran out of mana!");

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(this);
            }

            return false;
        }

        if (progress > lifetime)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(this);
            }

            return false;
        }

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnProgress(progress, this);
        }

        return true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            if (controller.Caster.AppliedStatusDict.TryGetValue(Status_Knockback.KnockbackKey, out StatusBase status) == true)
            {
                Status_Knockback k = status as Status_Knockback;

                Vector3 closestPoint = other.ClosestPoint(transform.position);

                Vector3 surfaceNormal = (closestPoint - transform.position).normalized;

                Vector3 bounce = Vector3.Reflect(k.KnockbackDirection, surfaceNormal);

                k.AddAdditionalKnockBackSpeed(bounce);

                k.reduceKnockBackSpeed(0.5f);

                hit.DirectDamage((int)k.KnockbackDirection.magnitude);//May change it to normal damage method later!

                foreach (AddedEffectSO added in addedEffects)
                {
                    added.OnTargetFound(hit, this);
                }
            }
        }

        if (other.TryGetComponent<Projectile>(out Projectile projectile) == true)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnProjectileFound(projectile, this);
            }
        }

        if (other.TryGetComponent<Melee>(out Melee melle) == true)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnMeleeFound(melle, this);
            }
        }

        if (other.TryGetComponent<Area>(out Area area) == true)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnAreaFound(area, this);
            }
        }
    }
}
