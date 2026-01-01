using UnityEngine;

public class Projectile_Controller : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private ElementType element;
    [Header("Visuals")]
    [SerializeField] private GameObject impactFxPrefab;
    [Header("Effect Stats")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float power = 2f;

    private Rigidbody2D rb;
    private float magicDamage;
    private bool hasHit;

    // 1. New Variable to store the Player
    private Transform owner;

    // 2. Update Setup to accept the Owner
    public void Setup(float _magicDamage, int _direction, Transform _owner)
    {
        rb = GetComponent<Rigidbody2D>();
        magicDamage = _magicDamage;
        owner = _owner; // Store the player reference

        rb.linearVelocity = new Vector2(speed * _direction, 0);
        if (_direction == -1) transform.rotation = Quaternion.Euler(0, 180, 0);
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        if (collision.GetComponent<Player>() != null) return;

        Entity target = collision.GetComponent<Entity>();
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null && target != null)
        {
            if (target.gameObject.layer == gameObject.layer) return;

            hasHit = true;

            // 1. Apply Status FIRST
            if (element == ElementType.Fire)
            {
                target.Ignite(duration, power);
            }
            else if (element == ElementType.Ice)
            {
                target.Chill(duration, power);
            }

            // --- THE FIX ---
            // Use 'owner' (Player) if it exists. Fallback to 'transform' (Projectile) only if necessary.
            // This ensures the Enemy targets YOU, not the fireball that is about to vanish.
            Transform damageSource = (owner != null) ? owner : transform;

            // 2. Deal Damage SECOND (Pass damageSource instead of transform)
            damageable.TakeDamage(0, magicDamage, damageSource, false, false);
            // ----------------

            FinalizeHit();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasHit = true;
            FinalizeHit();
        }
    }
    private void FinalizeHit()
    {
        if (impactFxPrefab != null) Instantiate(impactFxPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}