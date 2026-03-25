using System.Collections.Generic;
using UniGameMaths;
using UnityEngine;

public interface AreaController
{
    public void CheckProjectile(Projectile p);
    public void CheckMelee(Melee m);
    public void CheckSummon(SummonObject s);
}

public class Area : MonoBehaviour
{
    private ICombatObject controller;
    private AreaController areaController; //Should be same object as controller!

    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;

    [SerializeField, Range(0, 1)]
    private float progressSpeed = 1f;

    [SerializeField]
    private AddedEffectSO[] addedEffects;

    [SerializeField]
    private float startingScale, maxScale, lifeTime, scaleSpeed,damageRate;

    private HashSet<DroneUnitBody> overlapedTargets = new HashSet<DroneUnitBody>();
    private HashSet<Projectile> overlapedProjectiles = new HashSet<Projectile>();
    private HashSet<Melee> overlapedSlashes = new HashSet<Melee>();
    private HashSet<SummonObject> overlapedSummons = new HashSet<SummonObject>();

    private float startingMana;
    private float progress;
    private float currentScale;
    private float damagetime;


    [SerializeField]
    private bool isMagical = false;
    public bool isMagic { get { return isMagical; } }

    [SerializeField]
    private bool isDamaging = true;
    public bool dealsDamage { get { return isDamaging; } }
    public void InitArea(ICombatObject c, AreaController a)
    {
        controller = c;
        areaController = a;
    }

    public void PlaceArea(float mana, Vector3 pos)
    {
        transform.position = pos;
        currentScale = startingScale;
        transform.localScale = Vector3.one * currentScale;
        startingMana = mana;
        progress = 0f;

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnStarted(null,null,null,this);
        }
    }

    public bool SustainArea(Vector3 pos)
    {
        if (controller.Caster.AppliedStatusDict.TryGetValue(Status_Stunned.StunnedKey, out StatusBase status) == true)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, null, null, this);
            }

            return false;
        }

        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * progressSpeed;
        currentScale += scaleSpeed * Time.deltaTime;
        damagetime += Time.deltaTime;
        float modifier = Mathf.Clamp(currentScale,startingScale, maxScale);
        transform.localScale = new Vector3(1 * modifier,1 * modifier, 1 * modifier); 
        transform.position = pos;

        if (damagetime >= damageRate)
        {
            damagetime = 0;
            foreach (var target in overlapedTargets)
            {
                bool affected = controller.FinalEffectReturnValue(target);

                if (affected == true)
                {
                    CombatListener.AddLineToCombatText($"Area Damages {target.DroneUnit.DroneName} with {(int)startingMana} mana left!");

                    for (int i = 0; i < controller.Caster.MultiHits + 1; i++)
                    {
                        if (isDamaging == true)
                        {
                            target.TakeDamage((int)controller.myDamageType + (int)baseDamage, startingMana);
                        }
                        else
                        {
                            target.Heal((int)controller.myDamageType, startingMana);
                        }
                    }

                    foreach (AddedEffectSO added in addedEffects)
                    {
                        added.OnTargetFound(target, null, null, null, this);
                    }
                }
            }

            foreach (var target in overlapedProjectiles)
            {
                areaController.CheckProjectile(target);

                foreach (AddedEffectSO added in addedEffects)
                {
                    added.OnProjectileFound(target, null, null, null, this);
                }
            }

            foreach (var target in overlapedSlashes)
            {
                areaController.CheckMelee(target);

                foreach (AddedEffectSO added in addedEffects)
                {
                    added.OnMeleeFound(target, null, null, null, this);
                }
            }

            foreach (var target in overlapedSummons)
            {
                areaController.CheckSummon(target);

                foreach (AddedEffectSO added in addedEffects)
                {
                    added.OnSummonObjectFound(target, null, null, null, this);
                }
            }
        }

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Area ran out of mana!");

            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, null, null, this);
            }

            return false;
        }

        if (progress > lifeTime)
        {
            foreach (AddedEffectSO added in addedEffects)
            {
                added.OnCompleted(null, null, null, this);
            }

            return false;
        }

        foreach (AddedEffectSO added in addedEffects)
        {
            added.OnProgress(progress, null, null, null, this);
        }

        return true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            overlapedTargets.Add(hit);
        }

        if (other.TryGetComponent<Projectile>(out Projectile projectile) == true)
        {
            overlapedProjectiles.Add(projectile);
        }

        if (other.TryGetComponent<Melee>(out Melee slash) == true)
        {
            overlapedSlashes.Add(slash);
        }
        if (other.TryGetComponent<SummonObject>(out SummonObject summon) == true)
        {
            overlapedSummons.Add(summon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<DroneUnitBody>(out DroneUnitBody hit) == true)
        {
            overlapedTargets.Remove(hit);
        }

        if (other.TryGetComponent<Projectile>(out Projectile projectile) == true)
        {
            overlapedProjectiles.Remove(projectile);
        }

        if (other.TryGetComponent<Melee>(out Melee slash) == true)
        {
            overlapedSlashes.Remove(slash);
        }
        if (other.TryGetComponent<SummonObject>(out SummonObject summon) == true)
        {
            overlapedSummons.Remove(summon);
        }
    }
}
