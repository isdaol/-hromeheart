using UnityEngine;

public class BossShooter : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private BossStats stats;

    [Header("References")]
    public GameObject projectilePrefab;
    public Transform player;
    public Transform firePoint;

    private float timer;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("BossStats is not assigned.", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (player == null || projectilePrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= stats.fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        BossProjectile bullet =
            projectile.GetComponent<BossProjectile>();

        if (bullet != null)
            bullet.SetTarget(player);
    }
}