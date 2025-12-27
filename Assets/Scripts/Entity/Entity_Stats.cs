using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Ice,
    Lightning
}

public class Entity_Stats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stat strength;     // 1 Str = 1 Phy Dmg, 0.5% CritPower
    public Stat agility;      // 1 Agi = 0.5% Evasion, 0.3% CritChance
    public Stat intelligence; // 1 Int = 1 Magic Dmg, 0.5% Magic Res
    public Stat vitality;     // 1 Vit = 5 Health, 1 Armor

    [Header("Offensive Stats")]
    public Stat damage;       // Base Physical Damage
    public Stat critChance;
    public Stat critPower;    // Default 150%

    [Header("Elemental Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealth();
    }

    #region Physical Calculations

    public float GetTotalDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    public float GetTotalCritChance()
    {
        return critChance.GetValue() + (agility.GetValue() * 0.3f);
    }

    public float GetTotalCritPower()
    {
        return critPower.GetValue() + (strength.GetValue() * 0.5f);
    }

    #endregion

    #region Elemental Calculations

    public float GetTotalMagicDamage()
    {
        // 1. Get raw values
        float fire = fireDamage.GetValue();
        float ice = iceDamage.GetValue();
        float lightning = lightningDamage.GetValue();

        // 2. Find Dominant Element Value
        float dominantValue = Mathf.Max(fire, ice, lightning);

        // If no elemental damage exists, return 0
        if (dominantValue <= 0) return 0;

        // 3. Sum up the others (Total Sum - Dominant)
        float totalElementalSum = fire + ice + lightning;
        float otherElementsValue = totalElementalSum - dominantValue;

        // 4. Formula: Dominant(100%) + Others(50%) + Intelligence
        float finalMagicDamage = dominantValue + (otherElementsValue * 0.5f) + intelligence.GetValue();

        return finalMagicDamage;
    }

    public ElementType GetDominantElement()
    {
        float fire = fireDamage.GetValue();
        float ice = iceDamage.GetValue();
        float lightning = lightningDamage.GetValue();

        // If all are zero, no element
        if (fire == 0 && ice == 0 && lightning == 0) return ElementType.None;

        // Return the type with the highest value
        if (fire > ice && fire > lightning) return ElementType.Fire;
        if (ice > fire && ice > lightning) return ElementType.Ice;
        if (lightning > fire && lightning > ice) return ElementType.Lightning;

        // Fallback (e.g., if equal) -> Priority: Fire > Ice > Lightning
        if (fire >= ice && fire >= lightning) return ElementType.Fire;
        if (ice >= lightning) return ElementType.Ice;

        return ElementType.Lightning;
    }

    public float GetTotalMagicResistance()
    {
        // Base Magic Res + (Int * 0.5)
        return magicResistance.GetValue() + (intelligence.GetValue() * 0.5f);
    }

    #endregion

    #region Defensive Calculations

    public float GetMaxHealth()
    {
        return maxHealth.GetValue() + (vitality.GetValue() * 5f);
    }

    public float GetTotalArmor()
    {
        return armor.GetValue() + vitality.GetValue();
    }

    public float GetTotalEvasion()
    {
        return evasion.GetValue() + (agility.GetValue() * 0.5f);
    }

    #endregion

    // --- Health Logic Integration ---
    // Moving currentHealth here allows Stats to manage it directly
    [HideInInspector] public float currentHealth;

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
    }
}