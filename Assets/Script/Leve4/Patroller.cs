using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Patroller : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        AttackWindUp,
        Cooldown
    }

    [Header("References")]
    public Transform player;
    public Transform patrolPointA;
    public Transform patrolPointB;
    public LayerMask visionMask;

    [Header("Data")]
    [SerializeField] private PatrollerStats stats;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Transform currentPatrolTarget;
    private State currentState = State.Patrol;
    private float moveDirection;
    private float stateTimer;
    private float patrolWaitUntil;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("PatrollerStats is not assigned.", this);
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentPatrolTarget = patrolPointB;
        patrolWaitUntil = Time.time + stats.patrolWaitTime;
    }

    void Update()
    {
        ResolvePlayerReference();

        if (player == null)
        {
            currentState = State.Patrol;
            Patrol();
            UpdateAnimator();
            return;
        }

        bool canSeePlayer = CanSeePlayer();
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        switch (currentState)
        {
            case State.Patrol:
                if (canSeePlayer)
                {
                    if (distance <= stats.attackRange && CanAttackNow())
                        StartAttackWindUp();
                    else
                        currentState = State.Chase;
                }

                if (currentState == State.Patrol)
                    Patrol();
                else
                    moveDirection = 0f;
                break;

            case State.Chase:
                if (!canSeePlayer)
                {
                    currentState = State.Patrol;
                    Patrol();
                }
                else if (distance <= stats.attackRange && CanAttackNow())
                {
                    StartAttackWindUp();
                }
                else
                {
                    Chase();
                }
                break;

            case State.AttackWindUp:
                moveDirection = 0f;

                if (!canSeePlayer ||
                    distance > stats.attackRange ||
                    !EnemyScreenVisibility.IsOnScreen(spriteRenderer))
                {
                    currentState = canSeePlayer
                        ? State.Chase
                        : State.Patrol;

                    animator.ResetTrigger("Attack");
                    break;
                }

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    if (ApplyAttackDamage())
                    {
                        currentState = State.Cooldown;
                        stateTimer = stats.attackCooldown;
                    }
                    else
                    {
                        currentState = CanSeePlayer()
                            ? State.Chase
                            : State.Patrol;
                    }
                }

                break;

            case State.Cooldown:
                moveDirection = 0f;
                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    currentState = canSeePlayer
                        ? State.Chase
                        : State.Patrol;
                }

                break;
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (stats == null)
            return;

        float speed = currentState == State.Chase
            ? stats.chaseSpeed
            : stats.patrolSpeed;

        rb.linearVelocity = new Vector2(
            moveDirection * speed,
            rb.linearVelocity.y
        );
    }

    void ResolvePlayerReference()
    {
        if (player != null)
            return;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void Patrol()
    {
        if (patrolPointA == null || patrolPointB == null)
        {
            moveDirection = 0f;
            return;
        }

        if (Time.time < patrolWaitUntil)
        {
            moveDirection = 0f;
            return;
        }

        if (currentPatrolTarget == null)
            currentPatrolTarget = patrolPointB;

        float difference =
            currentPatrolTarget.position.x - transform.position.x;

        if (Mathf.Abs(difference) < stats.waypointArrivalTolerance)
        {
            currentPatrolTarget =
                currentPatrolTarget == patrolPointA
                    ? patrolPointB
                    : patrolPointA;

            patrolWaitUntil = Time.time + stats.patrolWaitTime;
            moveDirection = 0f;
            return;
        }

        moveDirection = Mathf.Sign(difference);
        FaceDirection(moveDirection);
    }

    void Chase()
    {
        float difference = player.position.x - transform.position.x;
        moveDirection = Mathf.Sign(difference);
        FaceDirection(moveDirection);
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 origin =
            transform.position + Vector3.up * stats.visionOriginOffset;

        Vector2 toPlayer = (Vector2)player.position - origin;
        float distance = toPlayer.magnitude;

        if (distance > stats.detectionRange)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            toPlayer.normalized,
            distance,
            visionMask
        );

        return hit.collider != null &&
               hit.collider.transform.root == player.root;
    }

    bool CanAttackNow()
    {
        return EnemyScreenVisibility.IsOnScreen(spriteRenderer);
    }

    void StartAttackWindUp()
    {
        currentState = State.AttackWindUp;
        stateTimer = stats.windUpTime;
        moveDirection = 0f;

        FaceDirection(
            Mathf.Sign(player.position.x - transform.position.x)
        );

        animator.SetTrigger("Attack");
    }

    bool ApplyAttackDamage()
    {
        if (player == null ||
            !CanSeePlayer() ||
            Vector2.Distance(transform.position, player.position) >
                stats.attackRange ||
            !EnemyScreenVisibility.IsOnScreen(spriteRenderer))
        {
            return false;
        }

        Health targetHealth = player.GetComponentInParent<Health>();

        if (targetHealth != null)
            targetHealth.TakeDamage(stats.attackDamage);

        return true;
    }

    void FaceDirection(float direction)
    {
        if (spriteRenderer != null && direction != 0f)
            spriteRenderer.flipX = direction < 0f;
    }

    void UpdateAnimator()
    {
        bool isMoving =
            currentState == State.Patrol ||
            currentState == State.Chase;

        float speed = currentState == State.Chase
            ? stats.chaseSpeed
            : stats.patrolSpeed;

        animator.SetFloat(
            "Speed",
            isMoving ? Mathf.Abs(moveDirection) * speed : 0f
        );

        animator.SetBool(
            "IsChasing",
            currentState == State.Chase
        );
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            stats.detectionRange
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            stats.attackRange
        );
    }
}