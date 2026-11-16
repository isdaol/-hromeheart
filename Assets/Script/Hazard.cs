using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected HazardStats stats;

    protected virtual void Start()
    {
        if (stats == null)
        {
            Debug.LogError("HazardStats is not assigned.", this);
            enabled = false;
            return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.down * stats.fallSpeed;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
                health.TakeDamage(stats.damage);

            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();

            if (health != null)
                health.TakeDamage(stats.damage);

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
}