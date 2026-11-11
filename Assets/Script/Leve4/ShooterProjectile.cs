using UnityEngine;

public class ShooterProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
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
        Transform targetTransform = target.transform;
        if (!targetTransform.CompareTag("Player") &&
            !targetTransform.root.CompareTag("Player"))
        {
            return;
        }

        Health health = target.GetComponentInParent<Health>();
        if (health != null)
            health.TakeDamage(damage);

        Destroy(gameObject);
    }
}
