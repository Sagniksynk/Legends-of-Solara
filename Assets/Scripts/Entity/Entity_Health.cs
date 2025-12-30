using System;
using Unity.Mathematics;
using UnityEngine;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private EntityHit_Vfx entityHit_Vfx;
    private Entity entity;
    private Entity_Stats stats;

    public event Action OnDie;
    public event Action OnHealthChanged;

    public bool isDead { get; private set; }

    [Header("Visuals")]
    [SerializeField] private GameObject floatingTextPrefab;

    [Header("Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyknockbackDuration = .5f;

    [Header("Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = .3f;

    protected virtual void Awake()
    {
        entityHit_Vfx = GetComponent<EntityHit_Vfx>();
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
    }

    protected virtual void Start()
    {
        // Stats now manages the health value, we just trigger the update for UI
        OnHealthChanged?.Invoke();
    }

    // Updated Interface Implementation
    public virtual void TakeDamage(float physicalDamage, float magicDamage, Transform attacker, bool isCritical)
    {
        if (isDead) return;

        // 1. Calculate Physical vs Armor
        float finalPhysical = physicalDamage - stats.GetTotalArmor();
        finalPhysical = Mathf.Clamp(finalPhysical, 0, float.MaxValue); // Can't deal negative damage

        // 2. Calculate Magic vs Magic Resistance
        // Note: Magic Resistance is often a percentage (e.g. 10 Int = 5% resist), 
        // or flat reduction. Based on your prompt "0.5% per point", it is percentage based.
        float resistancePercent = stats.GetTotalMagicResistance(); // e.g., 5.0 for 5%
        float finalMagic = magicDamage * (1 - (resistancePercent / 100f));
        finalMagic = Mathf.Clamp(finalMagic, 0, float.MaxValue);

        float totalDamage = finalPhysical + finalMagic;

        if (floatingTextPrefab != null)
        {
            ShowFloatingText(totalDamage, isCritical);
        }

        // 3. Apply Knockback & VFX
        Vector2 knockback = CalculateKnockback(totalDamage, attacker);
        float duration = CalculateDuration(totalDamage);

        entity?.RecieveKnockback(knockback, duration);
        entityHit_Vfx?.PlayVfx(isCritical);

        // 4. Reduce Health
        ReduceHealth(totalDamage);
    }

    private void ShowFloatingText(float damageAmount, bool isCrit)
    {
        Vector3 ramdomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.5f, 1f),0);
        GameObject textObj = Instantiate(floatingTextPrefab, transform.position + ramdomOffset, Quaternion.identity);
        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        if(floatingText != null)
        {
            floatingText.Setup(Mathf.RoundToInt(damageAmount).ToString(),isCrit);
        }
    }
    protected void ReduceHealth(float damage)
    {
        // We use the Stats script to track health now, or sync it here.
        // For simplicity, we can keep tracking it here or call stats.DecreaseHealth
        // Let's modify stats directly to keep one source of truth:
        stats.DecreaseHealth(damage);

        OnHealthChanged?.Invoke();

        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetHealthNormalized()
    {
        return stats.currentHealth / stats.GetMaxHealth();
    }

    protected virtual void Die()
    {
        isDead = true;
        OnDie?.Invoke();
        entity.Die();
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x = knockback.x * direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyknockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage) => damage / stats.GetMaxHealth() > heavyDamageThreshold;
}