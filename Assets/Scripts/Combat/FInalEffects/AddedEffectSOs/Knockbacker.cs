using UnityEngine;

[CreateAssetMenu(fileName = "Knockbacker", menuName = "Scriptable Objects/Knockbacker")]
public class Knockbacker : AddedEffectSO
{
    [SerializeField]
    private float addModifier = 1f;

    [SerializeField]
    private float reduceModifier = 1f;

    [SerializeField]
    private bool isReverse = false;

    [SerializeField]
    private bool addOriginDir = false;

    public override void OnAreaFound(Area areaTarget, SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        
    }

    public override void OnCompleted(SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        
    }

    public override void OnMeleeFound(Melee meleeTarget, SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        
    }

    public override void OnProgress(float currentProgress, SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
       
    }

    public override void OnProjectileFound(Projectile projectileTarget, SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        
    }

    public override void OnStarted(SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        
    }

    public override void OnSummonObjectFound(SummonObject summonObjectTarget, SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        
    }

    public override void OnTargetFound(DroneUnitBody target, SummonObject userSummon = null, Projectile userProjectile = null, Melee userMelee = null, Area userArea = null)
    {
        if (target.AppliedStatusDict.TryGetValue(Status_Knockback.KnockbackKey, out StatusBase status) == true)
        {
            Status_Knockback k = status as Status_Knockback;

            Vector3 userPos = Vector3.zero;

            userPos = userSummon != null ? userSummon.transform.position : userPos;

            userPos = userProjectile != null ? userProjectile.transform.position : userPos;

            userPos = userMelee != null ? userMelee.transform.position : userPos;

            userPos = userArea != null ? userArea.transform.position : userPos;

            Vector3 dir = Vector3.zero;

            if (isReverse == true)
            {
                dir = (userPos - target.transform.position).normalized;
            }
            else
            {
                dir = (target.transform.position - userPos).normalized;
            }

            dir = dir * addModifier;

            if (addOriginDir == true) dir = dir + k.KnockbackDirection;

            k.AddAdditionalKnockBackSpeed(dir);

            k.reduceKnockBackSpeed(reduceModifier);
        }
    }
}
