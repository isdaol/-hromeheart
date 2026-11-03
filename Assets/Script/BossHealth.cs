
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 30;
    public Image healthFill;
    public GameObject victoryOverlay;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(
            currentHealth - damage, 0, maxHealth
        );

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            LevelProgress.CompleteScene(SceneManager.GetActiveScene().name);
            if (victoryOverlay != null)
            {
                victoryOverlay.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Debug.LogError("Cannot show victory screen: VictoryOverlay is not assigned.");
            }

            Destroy(gameObject);
        }
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth / maxHealth;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
}
