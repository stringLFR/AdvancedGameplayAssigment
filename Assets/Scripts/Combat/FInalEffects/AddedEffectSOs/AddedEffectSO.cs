using UnityEngine;

[CreateAssetMenu(fileName = "AddedEffectSO", menuName = "Scriptable Objects/AddedEffectSO")]
public abstract class AddedEffectSO : ScriptableObject
{
    public abstract void OnProgress(float currentProgress,
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnCompleted(
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnStarted(
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnTargetFound(DroneUnitBody target, 
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnProjectileFound(Projectile projectileTarget, 
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnMeleeFound(Melee meleeTarget, 
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnAreaFound(Area areaTarget, 
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);

    public abstract void OnSummonObjectFound(SummonObject summonObjectTarget, 
        SummonObject userSummon = null, 
        Projectile userProjectile = null, 
        Melee userMelee = null, 
        Area userArea = null);
}
