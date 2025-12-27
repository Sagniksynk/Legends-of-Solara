using UnityEngine;

public interface IDamageable
{
    // Now accepts Magic Damage as a separate parameter
    void TakeDamage(float physicalDamage, float magicDamage, Transform attacker, bool isCritical);
}