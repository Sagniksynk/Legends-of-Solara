using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;

    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
    }
    public override void TakeDamage(float damage, Transform attacker)
    {
        base.TakeDamage(damage, attacker);
        if(enemy != null && !isDead)
        {
            enemy.DamageImpact(attacker);
        } 
    }
}
