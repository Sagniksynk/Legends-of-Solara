using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ArmorBar : MonoBehaviour
{
    [Header("Connections")]
    [Tooltip("Drag the Player GameObject here")]
    [SerializeField] private Entity_Stats stats;

    [Header("Visuals (Assign One)")]
    [Tooltip("Assign this if using your Neon/Liquid Shaders")]
    [SerializeField] private RawImage barRawImage;
    [Tooltip("Assign this if using a standard Unity Fill Image")]
    [SerializeField] private Image barImage;

    [Header("Optional Text")]
    [Tooltip("Displays 'Armor: 100 (50%)'")]
    [SerializeField] private TextMeshProUGUI armorText;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;

    // Internal
    private Material barMat;
    private int fillPropertyID;
    private float currentFill;

    private void Start()
    {
        // 1. Auto-Find Stats if missing
        if (stats == null)
            stats = GetComponentInParent<Entity_Stats>();

        // 2. Setup Shader Material (If using RawImage)
        if (barRawImage != null)
        {
            barMat = new Material(barRawImage.material);
            barRawImage.material = barMat;
            fillPropertyID = Shader.PropertyToID("_FillAmount");
        }
    }

    private void Update()
    {
        if (stats == null) return;

        // 1. Get the Mitigation Value (0.0 to 1.0)
        // This effectively represents "How close to 100% invincibility are we?"
        float targetFill = stats.GetPhysicalMitigation();

        // 2. Smooth Animation
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);

        // 3. Update Visuals
        if (barRawImage != null && barMat != null)
        {
            // Update Shader
            barMat.SetFloat(fillPropertyID, currentFill);
        }
        else if (barImage != null)
        {
            // Update Standard Image
            barImage.fillAmount = currentFill;
        }

        // 4. Update Text (Optional)
        if (armorText != null)
        {
            float totalArmor = stats.GetTotalArmor();
            // Shows: "100 Armor (50%)"
            armorText.text = $"{Mathf.RoundToInt(totalArmor)} Armor ({Mathf.RoundToInt(targetFill * 100)}%)";
        }
    }

    private void OnDestroy()
    {
        if (barMat != null) Destroy(barMat);
    }
}