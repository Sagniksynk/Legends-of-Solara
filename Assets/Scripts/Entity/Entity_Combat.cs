using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_Stats stats;
    private CinemachineImpulseSource impulseSource;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Combat Status")]
    [SerializeField] private float stunDuration = 1.5f;
    // FIXED: Lowered default values from (2,2) to (0.5, 0.5) for milder shake
    [SerializeField] private Vector3 critShakeVelocity = new Vector3(0.2f, 0.2f, 0);

    private void Start()
    {
        stats = GetComponent<Entity_Stats>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void PerformAttack()
    {
        Collider2D[] colliders = GetDetectedColliders();

        if (colliders.Length == 0) return;

        foreach (var collider in colliders)
        {
            Entity_Stats targetStats = collider.GetComponent<Entity_Stats>();
            IDamageable damageable = collider.GetComponent<IDamageable>();
            ICounterable counterable = collider.GetComponent<ICounterable>();

            bool isCounterAttack = false;

            if (counterable != null && counterable.CanBeCountered())
            {
                isCounterAttack = true;
                StartCoroutine(ApplyStunWithDelay(counterable, 0.15f));
            }

            if (!isCounterAttack && targetStats != null)
            {
                if (Random.Range(0, 100) < targetStats.GetTotalEvasion())
                {
                    Debug.Log("ATTACK BLOCKED/EVADED!");
                    continue;
                }
            }

            float physicalDamage = stats.GetTotalDamage();
            bool isCrit = false;

            if (Random.Range(0, 100) < stats.GetTotalCritChance())
            {
                isCrit = true;
                float critMultiplier = stats.GetTotalCritPower() / 100f;
                physicalDamage *= critMultiplier;
            }

            float magicDamage = stats.GetTotalMagicDamage();

            if (isCrit && impulseSource != null)
            {
                impulseSource.GenerateImpulseWithVelocity(critShakeVelocity);
            }

            if (damageable != null)
            {
                // FIXED: Passing 'isCounterAttack' so Entity_Health knows to apply Heavy Knockback
                damageable.TakeDamage(physicalDamage, magicDamage, transform, isCrit, isCounterAttack);
            }
        }
    }

    private IEnumerator ApplyStunWithDelay(ICounterable counterable, float delay)
    {
        yield return new WaitForSeconds(delay);
        counterable.StunFor(stunDuration);
    }

    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        if (targetCheck == null) return;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}