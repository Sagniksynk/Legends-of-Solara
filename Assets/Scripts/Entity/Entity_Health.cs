using System;
using UnityEngine;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private EntityHit_Vfx entityHit_Vfx;
    private Entity entity;
    private Entity_Stats stats;

    public event Action OnHealthChanged;
    public event Action OnDie; // Fixed event name order

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
        OnHealthChanged?.Invoke();
    }

    public virtual void TakeDamage(float physicalDamage, float magicDamage, Transform attacker, bool isCritical, bool isCounterAttack)
    {
        if (isDead) return;

        float armorPenetration = 0;
        float armorShred = 0;

        if (attacker != null)
        {
            Entity_Stats attackerStats = attacker.GetComponent<Entity_Stats>();
            if (attackerStats != null)
            {
                armorPenetration = attackerStats.GetArmorPenetration();
                armorShred = attackerStats.GetArmorShred();
            }
        }
        if (armorShred > 0)
        {
            stats.DamageArmor(armorShred);
        }

        // --- DAMAGE CALCULATIONS ---
        float physicalMitigation = stats.GetPhysicalMitigation(armorPenetration);
        float finalPhysical = physicalDamage * (1 - physicalMitigation);
        finalPhysical = Mathf.Clamp(finalPhysical, 0, float.MaxValue);

        float resistancePercent = stats.GetTotalMagicResistance();
        float finalMagic = magicDamage * (1 - (resistancePercent / 100f));
        finalMagic = Mathf.Clamp(finalMagic, 0, float.MaxValue);

        float totalDamage = finalPhysical + finalMagic;

        if (floatingTextPrefab != null)
        {
            ShowFloatingText(totalDamage, isCritical);
        }

         Vector2 knockback = CalculateKnockback(totalDamage, attacker, isCounterAttack);
         float duration = CalculateDuration(totalDamage, isCounterAttack);
         entity?.RecieveKnockback(knockback, duration);
        
        
        entityHit_Vfx?.PlayVfx(isCritical);

        ReduceHealth(totalDamage);

        
        
        

    }

    private void ShowFloatingText(float damageAmount, bool isCrit)
    {
        Vector3 ramdomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.5f, 1f), 0);
        GameObject textObj = Instantiate(floatingTextPrefab, transform.position + ramdomOffset, Quaternion.identity);
        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.Setup(Mathf.RoundToInt(damageAmount).ToString(), isCrit);
        }
    }

    protected void ReduceHealth(float damage)
    {
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

    private Vector2 CalculateKnockback(float damage, Transform damageDealer, bool isCounterAttack)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = (isCounterAttack || IsHeavyDamage(damage)) ? heavyKnockbackPower : knockbackPower;

        knockback.x = knockback.x * direction;
        return knockback;
    }

    private float CalculateDuration(float damage, bool isCounterAttack)
    {
        return (isCounterAttack || IsHeavyDamage(damage)) ? heavyknockbackDuration : knockbackDuration;
    }

    private bool IsHeavyDamage(float damage) => damage / stats.GetMaxHealth() > heavyDamageThreshold;
}