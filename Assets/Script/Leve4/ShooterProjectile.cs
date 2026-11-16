using UnityEngine;

public class ShooterProjectile : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ProjectileStats stats;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError(
                "ProjectileStats is not assigned to ShooterProjectile.",
                this
            );
            enabled = false;
            return;
        }

        Destroy(gameObject, stats.lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject target)
    {
        if (stats == null)
            return;

        Transform targetTransform = target.transform;

        if (!targetTransform.CompareTag("Player") &&
            !targetTransform.root.CompareTag("Player"))
        {
            return;
        }

        Health health = target.GetComponentInParent<Health>();

        if (health != null)
            health.TakeDamage(stats.damage);

        Destroy(gameObject);
    }
}