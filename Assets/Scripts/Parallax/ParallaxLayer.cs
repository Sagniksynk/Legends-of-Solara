using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax Effect")]
    [Tooltip("The parallax effect strength on each axis. 0 means no parallax, 1 means it moves with the player.")]
    [SerializeField] private Vector2 parallaxFactor;

    [Header("Scrolling Options")]
    [Tooltip("Check this for layers with a repeating pattern that should tile infinitely.")]
    [SerializeField] private bool infiniteScroll = true;
    
    [Tooltip("Check this for a single, non-repeating object (like a sky or moon) that should always stay in view.")]
    [SerializeField] private bool followCamera = false;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private Vector3 startPosition;
    private Vector3 cameraStartPosition;
    private float spriteWidth;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        startPosition = transform.position;
        cameraStartPosition = cameraTransform.position;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteWidth = spriteRenderer.bounds.size.x;
        }
    }

    void LateUpdate()
{
    if (followCamera)
    {
        // Mode 1: Follow Camera (for non-repeating backgrounds like the sky)
        Vector3 distanceMoved = cameraTransform.position - cameraStartPosition;
        transform.position = startPosition + new Vector3(distanceMoved.x * parallaxFactor.x, distanceMoved.y * parallaxFactor.y, 0);
    }
    else
    {
        // Mode 2: Frame-by-Frame movement (for tiling or finite backgrounds)
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxFactor.x, deltaMovement.y * parallaxFactor.y, 0);
        lastCameraPosition = cameraTransform.position;

        // --- New, More Robust Tiling Logic ---
        if (infiniteScroll && spriteWidth > 0)
        {
            // Check the distance between the camera and the center of this background layer
            float distance = cameraTransform.position.x - transform.position.x;

            // If the camera is more than half a sprite's width away, reposition the background
            if (distance > spriteWidth / 2)
            {
                // The camera is too far to the right, so move the background ahead of it.
                transform.position += new Vector3(spriteWidth, 0, 0);
            }
            else if (distance < -spriteWidth / 2)
            {
                // The camera is too far to the left, so move the background behind it.
                transform.position -= new Vector3(spriteWidth, 0, 0);
            }
        }
    }
}
}