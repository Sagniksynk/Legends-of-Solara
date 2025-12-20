using System;
using UnityEngine;

public class Entity_Stamina : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 20f;
    [SerializeField] private float regenDelay = 1.5f;

    [Header("Costs")]
    public float jumpCost = 15f;
    public float dashCost = 25f;
    public float attackCost = 10f;

    private float currentStamina;
    private float lastStaminaUseTime;

    public event Action OnStaminaChanged;

    private void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaUI();
    }
    private void Update()
    {
        if(Time.time >= lastStaminaUseTime + regenDelay)
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += regenRate * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
                UpdateStaminaUI();
            }
        } 
    }
    public bool TryConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            lastStaminaUseTime = Time.time;
            UpdateStaminaUI();
            return true;
        }
        else
        {
            return false;
        }
    }
    public float GetStaminaNormalised()
    {
        return currentStamina / maxStamina;
    }
    private void UpdateStaminaUI()
    {
        OnStaminaChanged?.Invoke();
    }
}
