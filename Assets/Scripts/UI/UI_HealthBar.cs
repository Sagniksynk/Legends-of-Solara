using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;

    [Header("Target Entity")]
    // 1. CHANGED: Now looks for Entity_Stats instead of Entity_Health
    [SerializeField] private Entity_Stats entityStats;

    [Header("Visibility Settings")]
    [SerializeField] private bool alwaysVisible = true;
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

        // 2. AUTO-FIND LOGIC (Updated type)
        entity = GetComponentInParent<Entity>();
        if (entityStats == null)
        {
            entityStats = GetComponentInParent<Entity_Stats>();
        }

        if (entityStats != null)
        {
            // 3. EVENT SUBSCRIPTION (Updated to match Entity_Stats event name)
            entityStats.onHealthChanged += OnHealthChanged;

            // 4. INITIAL VALUE
            slider.value = entityStats.GetHealthNormalized();
        }

        // HIDE ON START
        if (!alwaysVisible)
        {
            barContent.SetActive(false);
        }
        else
        {
            barContent.SetActive(true);
        }
    }

    private void OnHealthChanged()
    {
        // Show the bar because we were hit
        if (!alwaysVisible && entityStats.currentHealth > 0)
        {
            barContent.SetActive(true);
            visibleTimer = visibleDuration;
        }
    }

    private void Update()
    {
        if (entity != null) myTransform.rotation = Quaternion.identity;

        if (slider == null || entityStats == null) return;

        // 5. DEATH CHECK (Checked against health <= 0)
        if (entityStats.currentHealth <= 0)
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
        float targetHealth = entityStats.GetHealthNormalized();

        if (barContent.activeSelf)
        {
            slider.value = Mathf.Lerp(slider.value, targetHealth, Time.deltaTime * smoothingSpeed);
        }
    }

    private void OnDisable()
    {
        if (entityStats != null)
        {
            entityStats.onHealthChanged -= OnHealthChanged;
        }
    }
}