using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player2D : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerStats stats;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public AudioClip shootSound;
    public AudioClip coinSound;
    public GameObject victoryOverlay;

    private float nextShotTime;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool onGround;
    private int coins;
    private int coinsTotal;

    public int CurrentScore => coins;

    private Vector2 moveInput;
    private bool jumpRequested;
    private bool fireRequested;
    private bool completingLevel;
    private bool portalOpened;
    private bool exitedPortalAfterOpening;

    void Awake()
    {
        if (stats == null)
        {
            Debug.LogError("PlayerStats is not assigned.", this);
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        coinsTotal = GameObject.FindGameObjectsWithTag("Coin").Length;
        Debug.Log("Coins in this level: " + coinsTotal);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        jumpRequested = true;
    }

    public void OnFire()
    {
        fireRequested = true;
    }

    void Update()
    {
        if (completingLevel || Time.timeScale == 0f)
            return;

        float x = moveInput.x;

        if (x > 0)
            spriteRenderer.flipX = false;
        else if (x < 0)
            spriteRenderer.flipX = true;

        if (jumpRequested && onGround)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                stats.jumpForce
            );

            onGround = false;
            animator.SetBool("IsJumping", true);
            GameSounds.PlayJump();
        }

        jumpRequested = false;

        if (fireRequested && Time.time >= nextShotTime)
            Shoot();

        fireRequested = false;

        if (transform.position.y < -6f)
        {
            Debug.Log("Player fell!");
            GameSounds.RestartAfterDefeat();
        }
    }

    void FixedUpdate()
    {
        if (completingLevel)
            return;

        float x = moveInput.x;

        rb.linearVelocity = new Vector2(
            x * stats.speed,
            rb.linearVelocity.y
        );

        animator.SetFloat("Speed", Mathf.Abs(x));
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab is not assigned!");
            return;
        }

        bool facingLeft = spriteRenderer.flipX;

        Vector3 direction = facingLeft
            ? Vector3.left
            : Vector3.right;

        Vector3 spawnPosition =
            transform.position + direction * stats.bulletOffset;

        Quaternion rotation = facingLeft
            ? Quaternion.Euler(0f, 0f, 180f)
            : Quaternion.identity;

        Instantiate(bulletPrefab, spawnPosition, rotation);

        GameSounds.PlayGameplayEffect(shootSound);

        nextShotTime = Time.time + stats.shootCooldown;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            GameSounds.PlayGameplayEffect(coinSound);

            Destroy(other.gameObject);
            coins++;

            Debug.Log("Coins: " + coins + " / " + coinsTotal);
        }

        if (other.CompareTag("Exit"))
        {
            if (coins >= coinsTotal)
            {
                Debug.Log("All coins collected! Opening portal.");

                Animator portalAnimator =
                    other.GetComponent<Animator>();

                if (portalAnimator != null)
                    portalAnimator.SetBool("IsOpen", true);

                if (SceneManager.GetActiveScene().name == "Level4")
                {
                    ShowVictory();
                    return;
                }

                portalOpened = true;

                if (exitedPortalAfterOpening)
                    ShowVictory();
            }
            else
            {
                Debug.Log(
                    "Collect all coins first! " +
                    coins + " / " + coinsTotal
                );
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Exit") && portalOpened)
            exitedPortalAfterOpening = true;
    }

    void ShowVictory()
    {
        if (completingLevel)
            return;

        completingLevel = true;

        LevelProgress.CompleteScene(
            SceneManager.GetActiveScene().name
        );

        if (victoryOverlay == null)
        {
            Debug.LogError(
                "Cannot show victory screen: VictoryOverlay is not assigned."
            );
            completingLevel = false;
            return;
        }

        Time.timeScale = 0f;
        victoryOverlay.SetActive(true);
        GameSounds.PlayVictory();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main menu");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            animator.SetBool("IsJumping", false);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Game over");
            GameSounds.RestartAfterDefeat();
        }

        if (other.gameObject.CompareTag("Spikes"))
        {
            Debug.Log("You hit the spikes!");
            GameSounds.RestartAfterDefeat();
        }
    }
}
