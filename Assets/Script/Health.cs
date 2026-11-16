using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerStats stats;

    [Header("Heart UI")]
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Sounds")]
    public AudioClip damageSound;

    private int currentHealth;
    private bool isInvincible;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    public int CurrentHealth => currentHealth;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (stats == null)
        {
            Debug.LogError("PlayerStats is not assigned.", this);
            enabled = false;
            return;
        }

        currentHealth = stats.maxHealth;
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log(
            "HP: " + currentHealth + " / " + stats.maxHealth
        );

        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityCoroutine());
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(
            currentHealth + amount,
            stats.maxHealth
        );

        Debug.Log(
            "HP restored: " + currentHealth + " / " + stats.maxHealth
        );

        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (heartImages == null)
            return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            heartImages[i].sprite =
                i < currentHealth ? fullHeart : emptyHeart;
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < stats.invincibilityDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(stats.blinkInterval);
            elapsed += stats.blinkInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Game Over! Restarting level.");

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}