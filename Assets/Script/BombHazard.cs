using UnityEngine;

public class BombHazard : Hazard
{
    private bool hasLanded;
    private Rigidbody2D rb;

    protected override void Start()
    {
        if (stats == null)
        {
            Debug.LogError("HazardStats is not assigned.", this);
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.gravityScale = 1f;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLanded)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            GameSounds.WarnShot();
            Invoke(nameof(Explode), stats.explosionDelay);
        }
    }

    void Explode()
    {
        Debug.Log("BOOM!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            stats.explosionRadius
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health health = hit.GetComponent<Health>();

                if (health != null)
                    health.TakeDamage(stats.explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}
