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
    [SerializeField] private Vector3 critShakeVelocity = new Vector3(2f, 2f, 0);

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

            // --- 0. CHECK FOR COUNTER ---
            // We just flag it here, we don't apply Stun yet (Wait for knockback first)
            if (counterable != null && counterable.CanBeCountered())
            {
                isCounterAttack = true;
                // Start a delayed stun so the Knockback has time to apply force
                StartCoroutine(ApplyStunWithDelay(counterable, 0.15f));
            }

            // --- 1. Evasion Check ---
            // (Skip evasion check if we successfully Countered - Counters shouldn't miss!)
            if (!isCounterAttack && targetStats != null)
            {
                if (Random.Range(0, 100) < targetStats.GetTotalEvasion())
                {
                    Debug.Log("ATTACK BLOCKED/EVADED!");
                    continue;
                }
            }

            // --- 2. Calculate Damage ---
            float physicalDamage = stats.GetTotalDamage();
            bool isCrit = false;

            if (Random.Range(0, 100) < stats.GetTotalCritChance())
            {
                isCrit = true;
                float critMultiplier = stats.GetTotalCritPower() / 100f;
                physicalDamage *= critMultiplier;
            }

            float magicDamage = stats.GetTotalMagicDamage();

            // --- 3. Game Feel ---
            if (isCrit && impulseSource != null)
            {
                impulseSource.GenerateImpulseWithVelocity(critShakeVelocity);
            }

            // --- 4. Apply Damage & Knockback ---
            if (damageable != null)
            {
                // This applies the Force immediately
                damageable.TakeDamage(physicalDamage, magicDamage, transform, isCrit);
            }
        }
    }

    // --- NEW HELPER: Ensures Knockback happens before Stun Freezes them ---
    private IEnumerator ApplyStunWithDelay(ICounterable counterable, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for HitState/Knockback to start
        counterable.StunFor(stunDuration);      // NOW freeze them in StunState
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