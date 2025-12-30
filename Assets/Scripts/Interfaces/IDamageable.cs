using UnityEngine;

public interface IDamageable
{
    // Added 'isCounterAttack' parameter
    void TakeDamage(float physicalDamage, float magicDamage, Transform attacker, bool isCritical, bool isCounterAttack);
}