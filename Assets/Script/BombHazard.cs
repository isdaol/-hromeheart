
using UnityEngine;

public class BombHazard : Hazard
{
    public float explosionDelay = 2f;
    public float explosionRadius = 1.5f;
    public int explosionDamage = 1;

    private bool hasLanded;
    private Rigidbody2D rb;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
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

            Invoke(nameof(Explode), explosionDelay);
        }
    }

    void Explode()
    {
        Debug.Log("BOOM!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health health = hit.GetComponent<Health>();

                if (health != null)
                {
                    health.TakeDamage(explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }
}
