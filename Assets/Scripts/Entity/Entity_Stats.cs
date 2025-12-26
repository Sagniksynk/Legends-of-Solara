using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stat strength;     // 1 Str = 1 Dmg, 0.5% CritPower
    public Stat agility;      // 1 Agi = 0.5% Evasion, 0.3% CritChance
    public Stat intelligence; // 1 Int = 1 Magic Dmg, 0.5% Magic Res
    public Stat vitality;     // 1 Vit = 5 Health, 1 Armor

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;              // Default value usually 150% (1.5f) or similar

    [Header("Magic Stats")]
    public Stat magicDamage;
    public Stat magicResistance;

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;

    private void Start()
    {
        // Example: Initialize default crit power if not set in inspector
        // critPower.SetDefaultValue(150); 
    }

    #region Stat Calculations

    // 1. Strength Calculations
    // Strength gives +1 Physical Damage per point
    public float GetTotalDamage()
    {
        float damageFromStrength = strength.GetValue();
        return damage.GetValue() + damageFromStrength;
    }

    // Strength gives +0.5% Crit Power per point
    public float GetTotalCritPower()
    {
        float critPowerFromStr = strength.GetValue() * 0.5f;
        return critPower.GetValue() + critPowerFromStr;
    }

    // 2. Agility Calculations
    // Agility gives +0.5% Evasion per point
    public float GetTotalEvasion()
    {
        float evasionFromAgility = agility.GetValue() * 0.5f;
        return evasion.GetValue() + evasionFromAgility;
    }

    // Agility gives +0.3% Crit Chance per point
    public float GetTotalCritChance()
    {
        float critChanceFromAgility = agility.GetValue() * 0.3f;
        return critChance.GetValue() + critChanceFromAgility;
    }

    // 3. Intelligence Calculations
    // Intelligence gives +1 Magic Damage per point
    public float GetTotalMagicDamage()
    {
        float magicDamageFromInt = intelligence.GetValue();
        return magicDamage.GetValue() + magicDamageFromInt;
    }

    // Intelligence gives +0.5% Elemental Resistance per point
    public float GetTotalMagicResistance()
    {
        float magicResFromInt = intelligence.GetValue() * 0.5f;
        return magicResistance.GetValue() + magicResFromInt;
    }

    // 4. Vitality Calculations
    // Vitality gives +5 Max Health per point
    public float GetMaxHealth()
    {
        float healthFromVitality = vitality.GetValue() * 5f;
        return maxHealth.GetValue() + healthFromVitality;
    }

    // Vitality gives +1 Armor per point
    public float GetTotalArmor()
    {
        float armorFromVitality = vitality.GetValue();
        return armor.GetValue() + armorFromVitality;
    }

    #endregion
}