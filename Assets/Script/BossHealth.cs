using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private BossStats stats;

    [Header("UI")]
    public Image healthFill;
    public GameObject victoryOverlay;

    private int currentHealth;

    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("BossStats is not assigned.", this);
            enabled = false;
            return;
        }

        currentHealth = stats.maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(
            currentHealth - damage,
            0,
            stats.maxHealth
        );

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            LevelProgress.CompleteScene(
                SceneManager.GetActiveScene().name
            );

            if (victoryOverlay != null)
            {
                victoryOverlay.SetActive(true);
                Time.timeScale = 0f;
                GameSounds.PlayVictory();
            }
            else
            {
                Debug.LogError(
                    "Cannot show victory screen: VictoryOverlay is not assigned."
                );
            }

            Destroy(gameObject);
        }
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth / stats.maxHealth;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameSounds.RestartAfterDefeat();
        }
    }
}
