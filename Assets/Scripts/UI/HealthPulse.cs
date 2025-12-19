using UnityEngine;
using UnityEngine.UI;

public class HealthPulse : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseScale = 1.05f;
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30% health

    private Vector3 originalScale;

    private void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Only pulse if health is low
        if (slider.value <= lowHealthThreshold)
        {
            // Calculate a scale based on Sine wave
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f);
            transform.localScale = originalScale * scale;
        }
        else
        {
            // Return to normal size smoothly
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);
        }
    }
}