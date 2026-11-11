
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Charger : MonoBehaviour
{
    private enum State
    {
        Idle,
        WindUp,
        Charge,
        Return,
        Cooldown
    }

    [Header("References")]
    public Transform player;
    public Transform homePoint;

    [Header("Detection")]
    public float detectionRange = 7f;
    [Range(0f, 180f)]
    public float viewAngle = 60f;
    public LayerMask visionMask;

    [Header("Attack")]
    [Range(0.3f, 0.8f)] public float windUpTime = 0.5f;
    public float chargeSpeed = 9f;
    public float chargeDuration = 0.9f;
    public float cooldownTime = 1.5f;
    public int attackDamage = 1;

    [Header("Return")]
    public float returnSpeed = 2f;
    public float returnDistance = 0.15f;

    [Header("Idle Look")]
    public float turnInterval = 3f;

    [Header("Path Safety")]
    [Min(0.01f)] public float groundPathSampleStep = 0.2f;
    [Min(0f)] public float groundHeightTolerance = 0.45f;
    [Min(0f)] public float groundLookAheadDistance = 0.65f;
    [Min(0f)] public float edgeClearance = 0.1f;
    [Min(0f)] public float collisionPadding = 0.05f;
    [Min(0f)] public float minimumMovementThreshold = 0.01f;
    [Range(0f, 1f)] public float wallNormalThreshold = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D bodyCollider;

    private State currentState = State.Idle;

    private Vector2 homePosition;
    private Vector2 chargeDirection;

    private float facingDirection = 1f;
    private float stateTimer;
    private float nextTurnTime;
    private bool hasHitPlayerThisCharge;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bodyCollider = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.useFullKinematicContacts = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.linearVelocity = Vector2.zero;

        homePosition = homePoint != null
            ? (Vector2)homePoint.position
            : rb.position;
        nextTurnTime = Time.time + turnInterval;
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

        if ((currentState == State.Idle || currentState == State.Cooldown) &&
            Time.time >= nextTurnTime)
        {
            FaceDirection(-facingDirection);
            nextTurnTime = Time.time + turnInterval;
        }

        switch (currentState)
        {
            case State.Idle:

                if (CanSeePlayer() && IsOnScreen())
                    StartWindUp();

                break;

            case State.WindUp:

                if (!CanSeePlayer() || !IsOnScreen())
                {
                    currentState = State.Idle;
                    break;
                }

                FacePlayer();

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    if (CanSeePlayer() && IsOnScreen())
                        StartCharge();
                    else
                        currentState = State.Idle;
                }

                break;

            case State.Charge:

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                    StartReturn();

                break;

            case State.Return:

                float distance =
                    Vector2.Distance(rb.position, homePosition);

                if (distance <= returnDistance)
                {
                    rb.position = homePosition;
                    StartCooldown();
                }
                else
                {
                    FaceDirection(
                        Mathf.Sign(homePosition.x - rb.position.x)
                    );
                }

                break;

            case State.Cooldown:

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                    currentState = State.Idle;

                break;
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        bool canMoveByAI =
            currentState == State.Charge ||
            currentState == State.Return;

        rb.constraints = canMoveByAI
            ? RigidbodyConstraints2D.FreezeRotation
            : RigidbodyConstraints2D.FreezeAll;

        if (currentState == State.Charge)
        {
            float chargeStep = chargeSpeed * Time.fixedDeltaTime;
            if (!HasGroundAhead(chargeDirection.x) ||
                IsBlockedInDirection(chargeDirection, chargeStep + collisionPadding))
            {
                rb.linearVelocity = Vector2.zero;
                StartReturn();
                return;
            }

            rb.linearVelocity =
                chargeDirection * chargeSpeed;
        }
        else if (currentState == State.Return)
        {
            Vector2 direction =
                (homePosition - rb.position).normalized;

            if (Mathf.Abs(direction.x) > minimumMovementThreshold &&
                IsBlockedInDirection(
                    new Vector2(Mathf.Sign(direction.x), 0f),
                    returnSpeed * Time.fixedDeltaTime + collisionPadding
                ))
            {
                rb.linearVelocity = Vector2.zero;
                StartCooldown();
                return;
            }

            rb.linearVelocity =
                new Vector2(direction.x * returnSpeed,
                            rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 origin =
            transform.position + Vector3.up * 0.3f;

        Vector2 direction =
            ((Vector2)player.position - origin).normalized;

        float distance =
            Vector2.Distance(origin, player.position);

        if (distance > detectionRange)
            return false;

        Vector2 facing =
            new Vector2(facingDirection, 0f);

        if (Vector2.Angle(facing, direction) > viewAngle / 2f)
            return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            origin,
            direction,
            distance,
            visionMask
        );

        RaycastHit2D closestHit = default;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null ||
                hit.collider.transform.root == transform.root)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestHit = hit;
                closestDistance = hit.distance;
            }
        }

        return closestHit.collider != null &&
               closestHit.collider.transform.root == player.root &&
               HasGroundPathToPlayer();
    }

    bool HasGroundPathToPlayer()
    {
        if (bodyCollider == null || player == null)
            return true;

        float horizontalDifference = player.position.x - rb.position.x;
        float direction = Mathf.Sign(horizontalDifference);
        float chargeDistance = Mathf.Min(
            Mathf.Abs(horizontalDifference),
            chargeSpeed * chargeDuration
        );

        if (chargeDistance <= minimumMovementThreshold)
            return HasGroundAtX(rb.position.x);

        if (!HasGroundAtX(rb.position.x))
            return false;

        float leadingEdge = bodyCollider.bounds.extents.x + edgeClearance;
        for (float distance = Mathf.Min(leadingEdge, chargeDistance);
             distance <= chargeDistance;
             distance += groundPathSampleStep)
        {
            float sampleX = rb.position.x + direction * distance;
            if (!HasGroundAtX(sampleX))
                return false;
        }

        float targetX = rb.position.x + direction * chargeDistance;
        return HasGroundAtX(targetX);
    }

    bool HasGroundAhead(float direction)
    {
        if (bodyCollider == null)
            return true;

        float leadingEdge = bodyCollider.bounds.extents.x + edgeClearance;
        float endDistance = leadingEdge + groundLookAheadDistance;

        for (float distance = leadingEdge;
             distance <= endDistance;
             distance += groundPathSampleStep)
        {
            float sampleX = rb.position.x + direction * distance;
            if (!HasGroundAtX(sampleX))
                return false;
        }

        return true;
    }

    bool HasGroundAtX(float x)
    {
        float feetY = bodyCollider.bounds.min.y;
        Vector2 origin = new Vector2(
            x,
            bodyCollider.bounds.center.y
        );

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            origin,
            Vector2.down,
            bodyCollider.bounds.extents.y + groundHeightTolerance
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null &&
                !hit.collider.isTrigger &&
                hit.collider.CompareTag("Ground") &&
                Mathf.Abs(hit.point.y - feetY) <= groundHeightTolerance)
            {
                return true;
            }
        }

        return false;
    }

    bool IsBlockedInDirection(Vector2 direction, float distance)
    {
        if (bodyCollider == null || direction.sqrMagnitude <= minimumMovementThreshold * minimumMovementThreshold)
            return false;

        Bounds bounds = bodyCollider.bounds;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            bounds.center,
            bounds.size,
            0f,
            direction.normalized,
            distance
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || hit.collider.isTrigger ||
                hit.collider.transform.root == transform.root ||
                hit.collider.transform.root.CompareTag("Player"))
            {
                continue;
            }

            if (Mathf.Abs(hit.normal.x) > wallNormalThreshold)
                return true;
        }

        return false;
    }

    void StartWindUp()
    {
        currentState = State.WindUp;
        stateTimer = windUpTime;

        FacePlayer();
    }

    void StartCharge()
    {
        currentState = State.Charge;
        stateTimer = chargeDuration;
        hasHitPlayerThisCharge = false;

        chargeDirection =
            new Vector2(facingDirection, 0f);
    }

    void StartReturn()
    {
        currentState = State.Return;
    }

    void StartCooldown()
    {
        currentState = State.Cooldown;
        stateTimer = cooldownTime;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (currentState != State.Charge || hasHitPlayerThisCharge || !IsOnScreen())
            return;

        GameObject target = collision.gameObject;
        if (!target.CompareTag("Player"))
            return;

        Health health = target.GetComponentInParent<Health>();
        if (health == null)
            return;

        health.TakeDamage(attackDamage);
        hasHitPlayerThisCharge = true;
        StartReturn();
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        float direction =
            player.position.x - transform.position.x;

        if (direction != 0f)
            FaceDirection(Mathf.Sign(direction));
    }

    bool IsOnScreen()
    {
        return EnemyScreenVisibility.IsOnScreen(spriteRenderer);
    }

    void FaceDirection(float direction)
    {
        if (direction == 0f)
            return;

        facingDirection = direction;

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction < 0f;
    }

    void UpdateAnimator()
    {
        float speed =
            currentState == State.Return ? returnSpeed : 0f;

        animator.SetFloat("Speed", speed);

        animator.SetBool(
            "IsWindingUp",
            currentState == State.WindUp
        );

        animator.SetBool(
            "IsCharging",
            currentState == State.Charge
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
