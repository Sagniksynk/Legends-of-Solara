using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
    }

    // FIXED: Updated signature to include 'bool isCounterAttack'
    public override void TakeDamage(float physicalDamage, float magicDamage, Transform attacker, bool isCritical, bool isCounterAttack)
    {
        // Pass the new parameter to the base class
        base.TakeDamage(physicalDamage, magicDamage, attacker, isCritical, isCounterAttack);

        if (enemy != null && !isDead)
        {
            enemy.DamageImpact(attacker);
        }
    }
}