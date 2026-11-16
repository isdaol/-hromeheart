using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private BossStats stats;

    private Vector2 direction;

    public void SetTarget(Transform target)
    {
        if (target == null)
            return;

        direction = (
            (Vector2)target.position -
            (Vector2)transform.position
        ).normalized;
    }

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("BossStats is not assigned.", this);
            enabled = false;
            return;
        }

        Destroy(gameObject, stats.projectileLifetime);
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction * stats.projectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerHit(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerHit(collision.gameObject);
    }

    void HandlePlayerHit(GameObject target)
    {
        if (!target.CompareTag("Player"))
            return;

        Health health = target.GetComponentInParent<Health>();

        if (health != null)
            health.TakeDamage(stats.projectileDamage);

        Destroy(gameObject);
    }
}