using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
    }
    public override void TakeDamage(float physicalDamage, float magicDamage, Transform attacker, bool isCritical)
    {
        base.TakeDamage(physicalDamage,magicDamage, attacker,isCritical);
        if(enemy != null && !isDead)
        {
            enemy.DamageImpact(attacker);
        } 
    }
}
