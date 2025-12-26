using System;
using UnityEngine;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private EntityHit_Vfx entityHit_Vfx;
    private Entity entity;
    private Entity_Stats stats;

    public event Action OnDie;
    public event Action OnHealthChanged;

    [SerializeField] protected float maxHealth = 100;
    [SerializeField]protected float currentHealth;
    public bool isDead { get; private set; }
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
        currentHealth = stats.GetMaxHealth();
        //OnHealthChanged?.Invoke();
    }
    
    public virtual void TakeDamage(float damage, Transform attacker)
    {
        if(isDead) return;

        damage -= stats.GetTotalArmor();
        damage = Mathf.Clamp(damage,1,float.MaxValue);

        Vector2 knockback = CalculateKnockback(damage,attacker);
        float duration = CalculateDuration(damage);

        entity?.RecieveKnockback(knockback,duration);
        entityHit_Vfx?.PlayVfx();

        ReduceHealth(damage);
    }
    protected void ReduceHealth(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke();
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public float GetHealthNormalized()
    {
        return currentHealth/stats.GetMaxHealth();
    }

    protected virtual void Die()
    {
        isDead = true;
        OnDie?.Invoke();
        entity.Die();
    }
    private Vector2 CalculateKnockback(float damage,Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x = knockback.x * direction;
        return knockback;
    }
    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyknockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage) => damage / stats.GetMaxHealth() > heavyDamageThreshold;
}
