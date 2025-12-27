using System.Collections;
using UnityEngine;

public class EntityHit_Vfx : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] protected Material vfxMaterial;
    protected Material originaMaterial;
    [SerializeField] protected float vfxDuration = 0.15f;

    [Header("Hit Effects")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private GameObject criticalHitFxPrefab;
    [SerializeField] private Transform hitFxPosition;
    [SerializeField] private Vector2 randomHitOffset;

    private Coroutine onDamageVfxCoroutine;
    protected void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originaMaterial = spriteRenderer.material;
    }
    public virtual void PlayVfx(bool isCritical)
    {
        if(onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
        }
        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxPlay());
        GameObject prefabToSpawn = isCritical ? criticalHitFxPrefab : hitFxPrefab;
        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = hitFxPosition != null ? hitFxPosition.position : transform.position;
            float xOffset = Random.Range(-randomHitOffset.x, randomHitOffset.x);
            float yOffset = Random.Range(-randomHitOffset.y, randomHitOffset.y);

            spawnPos += new Vector3(xOffset, yOffset, 0);

            Quaternion randomRot = Quaternion.Euler(0,0,Random.Range(0,360));
            GameObject vfxPrefab =  Instantiate(prefabToSpawn, spawnPos, randomRot);
        }
    }
    private IEnumerator OnDamageVfxPlay()
    {
        spriteRenderer.material = vfxMaterial;
        yield return new WaitForSeconds(vfxDuration);
        spriteRenderer.material = originaMaterial;
    }
}
