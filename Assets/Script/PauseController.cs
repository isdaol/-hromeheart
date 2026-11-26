using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject pauseButton;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    public void Pause()
    {
        if (pauseOverlay == null)
        {
            Debug.LogError("Pause overlay is not assigned.", this);
            return;
        }

        pauseOverlay.SetActive(true);
        if (pauseButton != null)
            pauseButton.SetActive(false);
        Time.timeScale = 0f;
        GameSounds.SetPaused(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameSounds.SetPaused(false);

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameSounds.SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        GameSounds.SetPaused(false);
        SceneManager.LoadScene("Main menu");
    }
}
