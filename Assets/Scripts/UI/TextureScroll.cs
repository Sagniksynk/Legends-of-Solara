using UnityEngine;
using UnityEngine.UI;

public class TextureScroll : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float scrollSpeed = 0.5f; // Positive = Left, Negative = Right
    [SerializeField] private float tileWidth = 1.0f;   // How much to "squish" the texture

    private RawImage rawImage;
    private Rect currentUV;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        currentUV = rawImage.uvRect;
    }

    private void Update()
    {
        // 1. Calculate new position
        // We add speed * deltaTime to the X position
        currentUV.x -= scrollSpeed * Time.deltaTime;

        // 2. Keep the tiling size consistent
        currentUV.width = tileWidth;
        currentUV.height = 1f;

        // 3. Apply back to the image
        rawImage.uvRect = currentUV;
    }
}