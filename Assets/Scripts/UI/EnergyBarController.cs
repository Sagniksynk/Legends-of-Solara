using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RawImage))]
public class EnergyBarController : MonoBehaviour
{
    [Header("Connection")]
    public Player player; // <-- DRAG YOUR PLAYER HERE!

    [Range(0, 1)]
    public float fillAmount = 1.0f;

    [Header("Settings")]
    public float smoothSpeed = 10f; // Makes the bar move smoothly

    private RawImage _image;
    private Material _activeMaterial;
    private int _fillID;

    void OnEnable()
    {
        _image = GetComponent<RawImage>();
        _fillID = Shader.PropertyToID("_FillAmount");
        UpdateMaterial();
    }

    void Update()
    {
        if (_image == null) return;

        // 1. UPDATE VALUE FROM PLAYER (The missing link!)
        if (Application.isPlaying && player != null && player.stamina != null)
        {
            // Get the target (0.0 to 1.0)
            float targetFill = player.stamina.GetStaminaNormalised();

            // Smoothly move fillAmount towards the target
            fillAmount = Mathf.Lerp(fillAmount, targetFill, Time.deltaTime * smoothSpeed);
        }

        // 2. CHECK MATERIAL
        if (_activeMaterial != _image.materialForRendering)
        {
            UpdateMaterial();
        }

        // 3. PUSH TO SHADER
        if (_activeMaterial != null)
        {
            _activeMaterial.SetFloat(_fillID, fillAmount);
        }
    }

    void UpdateMaterial()
    {
        if (_image == null) return;
        _activeMaterial = _image.materialForRendering;
    }
}