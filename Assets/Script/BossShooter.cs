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
    private bool warningPlayed;

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
        if (Time.timeScale == 0f || player == null || projectilePrefab == null)
            return;

        timer += Time.deltaTime;

        float windUpTime = Mathf.Min(0.6f, stats.fireInterval / 2f);
        float warningStart = stats.fireInterval - windUpTime;
        if (!warningPlayed && timer >= warningStart)
        {
            timer = warningStart;
            warningPlayed = true;
            GameSounds.WarnShot();
            return;
        }

        if (timer >= stats.fireInterval)
        {
            Shoot();
            timer = 0f;
            warningPlayed = false;
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
