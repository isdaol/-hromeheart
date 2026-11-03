
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;

    private Vector2 direction;

    public void SetTarget(Transform target)
    {
        if (target == null) return;

        direction = ((Vector2)target.position -
                     (Vector2)transform.position).normalized;
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);
    }

    void Start()
    {
        Destroy(gameObject, 5f);
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
            health.TakeDamage(damage);

        Destroy(gameObject);
    }
}
