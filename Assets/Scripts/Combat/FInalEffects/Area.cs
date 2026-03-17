using System.Collections.Generic;
using UniGameMaths;
using UnityEngine;

public interface AreaController
{
    public void CheckProjectile(Projectile p);
    public void CheckMelee(Melee m);
}

public class Area : MonoBehaviour
{
    private ICombatObject controller;
    private AreaController areaController; //Should be same object as controller!

    [SerializeField]
    private float baseDamage = 1f, manaDrainPerSec = 1f;

    [SerializeField, Range(0, 1)]
    private float speed = 1f;

    [SerializeField]
    private float startingScale, maxScale, lifeTime, scaleSpeed;

    private HashSet<DroneUnitBody> overlapedTargets = new HashSet<DroneUnitBody>();
    private HashSet<Projectile> overlapedProjectiles = new HashSet<Projectile>();
    private HashSet<Melee> overlapedSlashes = new HashSet<Melee>();

    private float startingMana;
    private float progress;
    private float currentScale;

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
    }

    public bool SustainArea(Vector3 pos)
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;
        progress += Time.deltaTime * speed;
        currentScale += scaleSpeed * Time.deltaTime;
        transform.localScale = Vector3.one * Mathf.InverseLerp(startingScale, maxScale, EasingFunctionMaths.EaseInSine(currentScale));
        transform.position = pos;

        foreach (var target in overlapedTargets)
        {
            bool affected = controller.FinalEffectReturnValue(target);

            if (affected == true)
            {
                CombatListener.AddLineToCombatText($"Area Damages {target.DroneUnit.DroneName} with {(int)startingMana} mana left!");

                target.TakeDamage(controller.Caster.MyMelee_M_HitRate, startingMana);
            }
        }

        foreach (var target in overlapedProjectiles) areaController.CheckProjectile(target);

        foreach (var target in overlapedSlashes) areaController.CheckMelee(target);

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"Area ran out of mana!");
            return false;
        }

        if (progress > lifeTime) return false;

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
    }
}
