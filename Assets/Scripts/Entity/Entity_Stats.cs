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

    [Header("Formula Constants")]
    [Tooltip("The scaling constant for Armor calculation. Default 100.")]
    [SerializeField] private float armorScaling = 100f;

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
        float fire = fireDamage.GetValue();
        float ice = iceDamage.GetValue();
        float lightning = lightningDamage.GetValue();

        float dominantValue = Mathf.Max(fire, ice, lightning);

        if (dominantValue <= 0) return 0;

        float totalElementalSum = fire + ice + lightning;
        float otherElementsValue = totalElementalSum - dominantValue;

        float finalMagicDamage = dominantValue + (otherElementsValue * 0.5f) + intelligence.GetValue();

        return finalMagicDamage;
    }

    public ElementType GetDominantElement()
    {
        float fire = fireDamage.GetValue();
        float ice = iceDamage.GetValue();
        float lightning = lightningDamage.GetValue();

        if (fire == 0 && ice == 0 && lightning == 0) return ElementType.None;

        if (fire > ice && fire > lightning) return ElementType.Fire;
        if (ice > fire && ice > lightning) return ElementType.Ice;
        if (lightning > fire && lightning > ice) return ElementType.Lightning;

        if (fire >= ice && fire >= lightning) return ElementType.Fire;
        if (ice >= lightning) return ElementType.Ice;

        return ElementType.Lightning;
    }

    public float GetTotalMagicResistance()
    {
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
        // Vitality adds to Armor
        return armor.GetValue() + vitality.GetValue();
    }

    // --- NEW: Armor Mitigation Formula ---
    // Formula: Mitigation = Armor / (Armor + ScalingConstant)
    // Returns a float between 0.0 and 1.0 (e.g., 0.25 for 25% reduction)
    public float GetPhysicalMitigation()
    {
        float totalArmor = GetTotalArmor();

        // Prevent division by zero if using negative constants (unlikely but safe)
        float denominator = totalArmor + armorScaling;
        if (denominator == 0) return 0;

        float mitigation = totalArmor / denominator;

        // Clamp between 0 and 1 (0% to 100%)
        return Mathf.Clamp01(mitigation);
    }

    public float GetTotalEvasion()
    {
        float totalEvasion = evasion.GetValue() + (agility.GetValue() * 0.5f);

        // --- NEW: Cap at 90% ---
        return Mathf.Clamp(totalEvasion, 0, 90);
    }

    #endregion

    [HideInInspector] public float currentHealth;

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
    }
}