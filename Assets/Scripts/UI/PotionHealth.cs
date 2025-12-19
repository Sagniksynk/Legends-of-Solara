using UnityEngine;
using UnityEngine.UI;

public class PotionHealth : MonoBehaviour
{
    // Drag your Player (or whatever has health) here
    [Header("Connections")]
    [SerializeField] private Entity_Health targetHealth;

    // Drag the RawImage that has the shader material
    [SerializeField] private RawImage potionImage;

    // Optional: Smoothing speed
    [SerializeField] private float smoothSpeed = 5f;

    // Internal tracker
    private Material potionMat;
    private float currentFill = 1f;

    private void Start()
    {
        if (potionImage == null) potionImage = GetComponent<RawImage>();

        // --- THE FIX ---
        // Instead of grabbing the reference (which might be the asset), 
        // we force a NEW copy to be created in memory.
        potionMat = new Material(potionImage.material);

        // Assign this new copy back to the image so we see the changes
        potionImage.material = potionMat;
        // ----------------

        // Listen for changes
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateVisuals;
            currentFill = targetHealth.GetHealthNormalized();
            potionMat.SetFloat("_FillAmount", currentFill);
        }
    }

    private void UpdateVisuals()
    {
        // Just wakes up the update loop effectively
    }

    private void Update()
    {
        if (targetHealth == null) return;

        // 1. Get Target
        float targetFill = targetHealth.GetHealthNormalized();

        // 2. Smooth Move
        // If we are far from the target, lerp towards it
        if (Mathf.Abs(currentFill - targetFill) > 0.001f)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);

            // 3. Send to Shader
            // "_FillAmount" matches the name in your Shader Properties
            potionMat.SetFloat("_FillAmount", currentFill);
        }
    }

    private void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateVisuals;
        }
        // Clean up the material instance to prevent memory leaks
        if (potionMat != null)
        {
            Destroy(potionMat);
        }
    }
}