using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_Stats stats;

    [Header("Target Detection")]
    [SerializeField]private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Combat Status")]
    //[SerializeField] private float attackDamage = 10f;
    [SerializeField] private float stunDuration = 1.5f;

    private void Start()
    {
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        Collider2D[] colliders = GetDetectedColliders();
  
        foreach(var collider in colliders)
        {
            Entity_Stats targetStats = collider.GetComponent<Entity_Stats>();
            if (targetStats != null)
            {
                if (Random.Range(0, 100) < targetStats.GetTotalEvasion())
                {
                    Debug.Log("Attack Blocked");
                    continue;
                }
            }
            float finalDamage = stats.GetTotalDamage();

            if(Random.Range(0, 100) < stats.GetTotalCritChance())
            {
                float critMultiplier = stats.GetTotalCritPower()/100f;
                finalDamage *= critMultiplier;
                Debug.Log("Crit Hit");
            }

            ICounterable counterable = collider.GetComponent<ICounterable>();
            if (counterable != null && counterable.CanBeCountered())
            {
                counterable.StunFor(stunDuration);
                return;
            }
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable!=null)
            {
                damageable?.TakeDamage(finalDamage,transform);
            }
        }
    }
    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius,whatIsTarget);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
