using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D cd;
    private bool isOpen;
    private EntityHit_Vfx vfx => GetComponent<EntityHit_Vfx>();
    [Header("Open Details")]
    [SerializeField] private Vector2 knockback;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    // FIXED: Updated to match new Interface signature
    public void TakeDamage(float damage, float magicDamage, Transform attacker, bool isCritical)
    {
        if (isOpen) return;
        if (attacker.CompareTag("Player"))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isOpen = true;
        // FIXED: Added 'false' because a chest opening is never a "Critical Hit"
        vfx.PlayVfx(false);

        if (animator != null)
        {
            animator.SetBool("Open", true);
        }
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-100, 100);
    }
}