using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float fadeSpeed = 3f;

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        textColor = textMesh.color;
    }

    public void Setup(string text, bool isCritical)
    {
        textMesh.text = text;

        if (isCritical)
        {
            textMesh.fontSize *= 1.5f; // Make crit text bigger
            textMesh.color = Color.yellow; // Example crit color
            moveSpeed *= 1.2f;
        }
        else
        {
            textMesh.color = Color.white;
        }
    }

    private void Update()
    {
        // 1. Move Up (Using Unscaled time to ignore Hit Stop freeze)
        transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;

        // 2. Timer
        timer += Time.unscaledDeltaTime;

        // 3. Fade Out
        if (timer >= lifeTime - 0.5f) // Start fading near the end
        {
            float alpha = textMesh.color.a - (fadeSpeed * Time.unscaledDeltaTime);
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

            if (textMesh.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }

        // Hard destroy if fade fails
        if (timer >= lifeTime) Destroy(gameObject);
    }
}