using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField]private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Combat Status")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float stunDuration = 1.5f;
    public void PerformAttack()
    {
        Collider2D[] colliders = GetDetectedColliders();
        foreach(var collider in colliders)
        {
            ICounterable counterable = collider.GetComponent<ICounterable>();
            if (counterable != null && counterable.CanBeCountered())
            {
                counterable.StunFor(stunDuration);
                return;
            }
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable!=null)
            {
                damageable?.TakeDamage(attackDamage,transform);
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
