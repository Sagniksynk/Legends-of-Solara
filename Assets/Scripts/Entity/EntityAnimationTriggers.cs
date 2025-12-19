using UnityEngine;

public class EntityAnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;
    private Enemy_Vfx enemyVfx;
    void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
        enemyVfx = GetComponentInParent<Enemy_Vfx>();
    }
    public void AttackOver()
    {
        entity.CallAnimationTrigger();
    }
    public void AttackTrigger()
    {
        if(entityCombat != null)
        {
            entityCombat.PerformAttack();
        }
    }
    public void OpenCounterAttackWindow()
    {
        Enemy_Skeleton skeleton = entity as Enemy_Skeleton;
        if (skeleton != null)
        {
            enemyVfx.EnableAttackAlert(true);
            skeleton.OpenCounterAttackWindow();
        }
    }

    public void CloseCounterAttackWindow()
    {
        Enemy_Skeleton skeleton = entity as Enemy_Skeleton;
        if (skeleton != null)
        {
            enemyVfx.EnableAttackAlert(false);
            skeleton.CloseCounterAttackWindow();
        }
    }
}
