using UnityEngine;

public class Player_MagicController : MonoBehaviour
{
    private Entity_Stats stats;
    private Player player;

    [Header("Spells")]
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private GameObject iceShardPrefab;

    [Header("Mana Costs")]
    [SerializeField] private float fireCost = 10;
    [SerializeField] private float iceCost = 15;

    private ElementType currentSpell = ElementType.Fire;

    void Start()
    {
        stats = GetComponent<Entity_Stats>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        // 1. Switching
        if (Input.GetKeyDown(KeyCode.Alpha1)) { currentSpell = ElementType.Fire; Debug.Log("Equipped: Fire"); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { currentSpell = ElementType.Ice; Debug.Log("Equipped: Ice"); }

        // 2. Casting (Press F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptCast();
        }
    }

    private void AttemptCast()
    {
        float cost = (currentSpell == ElementType.Fire) ? fireCost : iceCost;

        if (stats.HasEnoughMana(currentSpell, cost))
        {
            // Correct way: Only switch state. 
            // The State's Enter() method will call ConsumeMana().
            player.stateMachine.ChangeState(player.spellCastState);
        }
        else
        {
            Debug.Log("Not enough Mana!");
        }
    }
    // Add this public function so the State Machine can call it
    public void ConsumeMana()
    {
        float cost = (currentSpell == ElementType.Fire) ? fireCost : iceCost;
        if (stats != null)
        {
            stats.UseMana(currentSpell, cost);
        }
    }
    // Called by Animation Event or AttemptCast
    public void CastSpell()
    {
        GameObject prefab = (currentSpell == ElementType.Fire) ? fireBallPrefab : iceShardPrefab;
        float xOffset = player.facingDirection * 1.5f;
        float yOffset = 0.5f;
        Vector3 spawnPosition = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, 0);

        GameObject spell = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Projectile_Controller script = spell.GetComponent<Projectile_Controller>();

        float damage = stats.GetTotalMagicDamage();

        // --- THE FIX: Pass 'transform' (Player) as the 3rd argument ---
        script.Setup(damage, player.facingDirection, transform);
    }
}