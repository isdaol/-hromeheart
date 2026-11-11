
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Shooter : MonoBehaviour
{
    private enum State
    {
        Idle,
        Aim,
        Shoot
    }

    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Detection")]
    public float detectionRange = 8f;
    [Range(0f, 180f)]
    public float viewAngle = 140f;
    public LayerMask visionMask;

    [Header("Shooting")]
    [Range(0.3f, 0.8f)] public float aimTime = 0.6f;
    public float cooldown = 1.6f;
    public float shootAnimationTime = 0.75f;
    public float projectileSpeed = 8f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private State currentState = State.Idle;

    private float aimTimer;
    private float shootTimer;
    private float nextShotTime;

    private float facingDirection = 1f;
    private float firePointX;

    private bool targetVisible;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (firePoint != null)
            firePointX = Mathf.Abs(firePoint.localPosition.x);
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        targetVisible = CanSeePlayer();

        switch (currentState)
        {
            case State.Idle:

                if (targetVisible && IsOnScreen())
                    StartAiming();

                break;

            case State.Aim:

                if (!targetVisible || !IsOnScreen())
                {
                    ReturnToIdle();
                    break;
                }

                FacePlayer();

                aimTimer -= Time.deltaTime;

                if (aimTimer <= 0f &&
                    Time.time >= nextShotTime)
                {
                    StartShooting();
                }

                break;

            case State.Shoot:

                shootTimer -= Time.deltaTime;

                if (shootTimer <= 0f)
                {
                    if (targetVisible)
                        currentState = State.Aim;
                    else
                        ReturnToIdle();
                }

                break;
        }

        UpdateAnimator();
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 origin = transform.position;
        Vector2 direction =
            (Vector2)player.position - origin;

        float distance = direction.magnitude;

        if (distance > detectionRange)
            return false;

        direction.Normalize();

        // Проверяем угол в обе стороны по горизонтали.
        float horizontalAngle =
            Vector2.Angle(Vector2.right, direction);

        if (horizontalAngle > 90f)
            horizontalAngle = 180f - horizontalAngle;

        if (horizontalAngle > viewAngle / 2f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            visionMask
        );

        return hit.collider != null &&
               hit.collider.transform.root == player.root;
    }

    void StartAiming()
    {
        currentState = State.Aim;
        aimTimer = aimTime;

        FacePlayer();
    }

    void StartShooting()
    {
        if (!targetVisible || !IsOnScreen())
        {
            ReturnToIdle();
            return;
        }

        currentState = State.Shoot;
        shootTimer = shootAnimationTime;

        animator.SetTrigger("Shoot");

        FireProjectile();

        nextShotTime = Time.time + cooldown;
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "Shooter: Assign Projectile Prefab and FirePoint!",
                this
            );
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        SpriteRenderer projectileRenderer =
            projectile.GetComponent<SpriteRenderer>();

        if (projectileRenderer != null)
            projectileRenderer.flipX = facingDirection < 0f;

        Rigidbody2D projectileRb =
            projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity =
                Vector2.right * facingDirection * projectileSpeed;
        }
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        float direction =
            player.position.x - transform.position.x;

        if (direction == 0f)
            return;

        facingDirection = Mathf.Sign(direction);

        if (spriteRenderer != null)
            spriteRenderer.flipX = facingDirection < 0f;

        if (firePoint != null)
        {
            Vector3 position = firePoint.localPosition;

            position.x = firePointX * facingDirection;

            firePoint.localPosition = position;
        }
    }

    bool IsOnScreen()
    {
        return EnemyScreenVisibility.IsOnScreen(spriteRenderer);
    }

    void ReturnToIdle()
    {
        currentState = State.Idle;
        animator.ResetTrigger("Shoot");
    }

    void UpdateAnimator()
    {
        animator.SetBool(
            "IsAiming",
            currentState == State.Aim ||
            currentState == State.Shoot
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
