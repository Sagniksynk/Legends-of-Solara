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
    public System.Action onHealthChanged;
    [Header("Major Stats")]
    public Stat strength;     // 1 Str = 1 Phy Dmg, 0.5% CritPower
    public Stat agility;      // 1 Agi = 0.5% Evasion, 0.3% CritChance
    public Stat intelligence; // 1 Int = 1 Magic Dmg, 0.5% Magic Res
    public Stat vitality;     // 1 Vit = 5 Health, 1 Armor

    [Header("Offensive Stats")]
    public Stat damage;       // Base Physical Damage
    public Stat critChance;
    public Stat critPower;    // Default 150%

    [Header("Attack Modifiers")]
    [Tooltip("Percent of target armor ignored")]
    public Stat armorPenetration;
    [Tooltip("Amount of armor destroyed per hit")]
    public Stat armorShred;

    [Header("Elemental Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    [Header("Elemental Mana Pools")]
    public float maxFireMana = 50f;
    public float currentFireMana;

    public float maxIceMana = 50f;
    public float currentIceMana;

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Formula Constants")]
    [Tooltip("The scaling constant for Armor calculation. Default 100.")]
    [SerializeField] private float armorScaling = 100f;

    //Internal variable to track broken armor
    private float currentArmorDamage = 0f;
    private Entity entity;

    protected virtual void Start()
    {
        entity = GetComponent<Entity>();
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealth();
        currentFireMana = maxFireMana;
        currentIceMana = maxIceMana;
    }

    #region Physical Calculations

    public float GetTotalDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    public float GetArmorPenetration() => armorPenetration.GetValue();
    public float GetArmorShred() => armorShred.GetValue();

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
    public void UseMana(ElementType type, float amount)
    {
        if(type == ElementType.Fire)
        {
            currentFireMana -= amount;
            if(currentFireMana < 0) currentFireMana = 0;
        }
        else if(type == ElementType.Ice)
        {
            currentIceMana -= amount;
            if( currentIceMana < 0) currentIceMana = 0;
        }
    }

    public bool HasEnoughMana(ElementType type, float amount)
    {
        if(type==ElementType.Fire) return currentFireMana >= amount;
        if(type==ElementType.Ice) return currentIceMana >= amount;
        return false;
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
        float total = armor.GetValue() + vitality.GetValue() - currentArmorDamage;
        return Mathf.Clamp(total, 0, float.MaxValue);
    }

    // --- NEW: Armor Mitigation Formula ---
    // Formula: Mitigation = Armor / (Armor + ScalingConstant)
    // Returns a float between 0.0 and 1.0 (e.g., 0.25 for 25% reduction)
    public float GetPhysicalMitigation(float attackerPenetration=0)
    {
        float totalArmor = GetTotalArmor();

        // Prevent division by zero if using negative constants (unlikely but safe)
        float effectiveArmor = totalArmor * (1f - Mathf.Clamp01(attackerPenetration / 100f));

        float denominator = effectiveArmor + armorScaling;
        if (denominator == 0) return 0;

        return Mathf.Clamp01(effectiveArmor / denominator);
    }

    public float GetTotalEvasion()
    {
        float totalEvasion = evasion.GetValue() + (agility.GetValue() * 0.5f);

        // --- NEW: Cap at 90% ---
        return Mathf.Clamp(totalEvasion, 0, 90);
    }
    public void DamageArmor(float amount)
    {
        if (amount <= 0) return;
        currentArmorDamage += amount;
        // Optional: Cap the shred? E.g., cannot shred below 0 base armor
        // For now, infinite shredding is fine as GetTotalArmor clamps to 0.
    }

    #endregion

    [HideInInspector] public float currentHealth;

    public float GetHealthNormalized()
    {
        if (GetMaxHealth() == 0) return 1;
        return currentHealth / GetMaxHealth();
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // --- CHANGE 3: Trigger Event & Death ---
        // 1. Tell the UI to update!
        if (onHealthChanged != null)
            onHealthChanged();

        // 2. If health drops to zero from DOT, we must manually trigger death
        if (currentHealth <= 0 && entity != null)
        {
            entity.Die();
        }
        // --------------------------------------
    }
}