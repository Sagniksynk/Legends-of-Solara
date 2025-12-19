using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;

    [Header("Target Entity")]
    [SerializeField] private Entity_Health entityHealth;

    [Header("Visibility Settings")]
    [SerializeField] private bool alwaysVisible = true; // TRUE for Player, FALSE for Enemy
    [SerializeField] private float visibleDuration = 3f;
    private float visibleTimer;

    [Header("Smoothing")]
    [SerializeField] private float smoothingSpeed = 10f;

    private Slider slider;
    private Transform myTransform;
    private GameObject barContent;

    private void Start()
    {
        myTransform = transform;
        slider = GetComponentInChildren<Slider>();
        barContent = slider.gameObject;

        // 1. AUTO-FIND LOGIC
        if (entityHealth == null)
        {
            entity = GetComponentInParent<Entity>();
            entityHealth = GetComponentInParent<Entity_Health>();
        }

        if (entityHealth != null)
        {
            entityHealth.OnHealthChanged += OnHealthChanged;

            // 2. SET INITIAL VALUE SILENTLY
            // We set the value directly without triggering the "Show" timer
            slider.value = entityHealth.GetHealthNormalized();
        }

        // 3. HIDE ON START (If enemy)
        if (!alwaysVisible)
        {
            barContent.SetActive(false);
        }
        else
        {
            barContent.SetActive(true);
        }
    }

    // This function runs ONLY when the Entity gets hit (TakeDamage -> ReduceHealth)
    private void OnHealthChanged()
    {
        UpdateHealthValue();

        // Show the bar because we were hit
        if (!alwaysVisible && !entityHealth.isDead)
        {
            barContent.SetActive(true);
            visibleTimer = visibleDuration;
        }
    }

    private void UpdateHealthValue()
    {
        // Just updates the data, doesn't touch visibility
    }

    private void Update()
    {
        if (entity != null) myTransform.rotation = Quaternion.identity;

        if (slider == null || entityHealth == null) return;

        // DEATH CHECK
        if (entityHealth.isDead)
        {
            barContent.SetActive(false);
            return;
        }

        // VISIBILITY TIMER
        if (!alwaysVisible && barContent.activeSelf)
        {
            visibleTimer -= Time.deltaTime;
            if (visibleTimer <= 0)
            {
                barContent.SetActive(false);
            }
        }

        // SMOOTHING
        float targetHealth = entityHealth.GetHealthNormalized();

        // Only Lerp if the bar is actually visible to save performance
        if (barContent.activeSelf)
        {
            slider.value = Mathf.Lerp(slider.value, targetHealth, Time.deltaTime * smoothingSpeed);
        }
    }

    private void OnDisable()
    {
        if (entityHealth != null)
        {
            entityHealth.OnHealthChanged -= OnHealthChanged;
        }
    }
}