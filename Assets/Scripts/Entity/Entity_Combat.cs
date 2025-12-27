using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_Stats stats;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Combat Status")]
    [SerializeField] private float stunDuration = 1.5f;

    private void Start()
    {
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        Collider2D[] colliders = GetDetectedColliders();
        foreach (var collider in colliders)
        {
            Entity_Stats targetStats = collider.GetComponent<Entity_Stats>();

            // --- 1. Evasion Check ---
            if (targetStats != null)
            {
                if (Random.Range(0, 100) < targetStats.GetTotalEvasion())
                {
                    Debug.Log("ATTACK EVADED!");
                    continue;
                }
            }

            // --- 2. Physical Damage Calculation ---
            float physicalDamage = stats.GetTotalDamage();

            // Critical Hit Logic (Affects Physical Only?) 
            // Usually in RPGs, Magic can crit too, but per your prompt: 
            // "Crit power Used to increase PHYSICAL damage when a critical hit occurs"
            bool isCrit = false;
            if (Random.Range(0, 100) < stats.GetTotalCritChance())
            {
                isCrit = true;
                float critMultiplier = stats.GetTotalCritPower() / 100f;
                physicalDamage *= critMultiplier;
                Debug.Log("CRITICAL HIT!");
            }

            // --- 3. Elemental Damage Calculation ---
            float magicDamage = stats.GetTotalMagicDamage();
            ElementType currentElement = stats.GetDominantElement();

            // --- 4. Apply Status Effects ---
            // (Placeholder for now until Status System is built)
            if (magicDamage > 0 && currentElement != ElementType.None)
            {
                // Logic to apply status effect will go here.
                // Example: collider.GetComponent<Entity_Status>().ApplyStatus(currentElement);
                // Debug.Log($"Applied Element: {currentElement}");
            }

            // --- 5. Deal Damage ---
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(physicalDamage, magicDamage, transform,isCrit);
            }
        }
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