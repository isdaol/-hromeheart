
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform player;
    public Transform firePoint;

    public float fireInterval = 2f;
    private float timer;

    void Update()
    {
        if (player == null || projectilePrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= fireInterval)
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
