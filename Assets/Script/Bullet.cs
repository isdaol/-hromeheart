using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ProjectileStats stats;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("ProjectileStats is not assigned.", this);
            enabled = false;
            return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = transform.right * stats.speed;
        }

        Destroy(gameObject, stats.lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        BossHealth boss = other.GetComponentInParent<BossHealth>();

        if (boss != null)
        {
            boss.TakeDamage(stats.damage);
            Destroy(gameObject);
        }
    }
}