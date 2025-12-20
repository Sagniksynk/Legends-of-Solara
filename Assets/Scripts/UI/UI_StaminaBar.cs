using UnityEngine;
using UnityEngine.UI;

public class UI_StaminaBar : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private Player player;     // Drag your Player here
    [SerializeField] private RawImage barImage; // Drag this GameObject's RawImage here

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    // Internal variables
    private Material barMat;
    private float currentFill = 1f;
    private int fillPropertyID; // Optimization ID

    private void Start()
    {
        if (barImage == null) barImage = GetComponent<RawImage>();

        // 1. Create a Material Instance
        // Important: This creates a copy so we don't modify the asset file on disk
        barMat = new Material(barImage.material);
        barImage.material = barMat;

        // 2. Cache Shader Property ID (Faster than using string every frame)
        fillPropertyID = Shader.PropertyToID("_FillAmount");

        // 3. Initial Setup
        if (player != null && player.stamina != null)
        {
            UpdateFill(true);
        }
    }

    private void Update()
    {
        if (player == null || player.stamina == null) return;

        UpdateFill(false);
    }

    private void UpdateFill(bool instant)
    {
        // Get target value (0 to 1) from your Stamina System
        float targetFill = player.stamina.GetStaminaNormalised();

        if (instant)
        {
            currentFill = targetFill;
        }
        else
        {
            // Smoothly interpolate for visual flair
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        }

        // Send value to the Shader
        barMat.SetFloat(fillPropertyID, currentFill);
    }

    // Cleanup memory when object is destroyed
    private void OnDestroy()
    {
        if (barMat != null) Destroy(barMat);
    }
}