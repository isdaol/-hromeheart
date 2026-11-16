using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject levelsPanel;

    [Header("Levels")]
    public LevelData[] levels;
    public Button[] levelButtons;

    [Header("Music")]
    public AudioSource bgSound;
    public AudioClip bgMusicClip;

    void Awake()
    {
        LevelProgress.Configure(levels);

        if (bgSound != null)
            bgSound.playOnAwake = false;

        BackgroundMusic.EnsurePlaying(
            bgMusicClip != null
                ? bgMusicClip
                : (bgSound != null ? bgSound.clip : null)
        );
    }

    void Start()
    {
        Time.timeScale = 1f;
        RefreshLevelButtons();

        if (PlayerPrefs.GetInt("OpenLevelsOnMenuStart", 0) == 1)
        {
            PlayerPrefs.DeleteKey("OpenLevelsOnMenuStart");
            OpenLevels();
        }
    }

    void RefreshLevelButtons()
    {
        if (levelButtons == null)
            return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable =
                    LevelProgress.IsUnlocked(i);
            }
        }
    }

    public void Play()
    {
        OpenLevels();
    }

    public void OpenLevels()
    {
        RefreshLevelButtons();

        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (levelsPanel != null)
            levelsPanel.SetActive(true);
    }

    public void CloseLevels()
    {
        if (levelsPanel != null)
            levelsPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    public void LoadLevel(int levelIndex)
    {
        if (!LevelProgress.IsUnlocked(levelIndex))
            return;

        string sceneName = LevelProgress.GetSceneName(levelIndex);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError(
                "The selected LevelData has no scene name.",
                this
            );
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting the game");
    }

    public void Settings()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void Help()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (helpPanel != null)
            helpPanel.SetActive(true);
    }

    public void BackFromSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    public void BackFromHelp()
    {
        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    public void SetVolume(float value)
    {
        BackgroundMusic.SetVolume(value);
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ResetProgressAndClosePanel(GameObject confirmPanel)
    {
        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager data is not available.", this);
            return;
        }

        saveManager.ResetProgress();
        RefreshLevelButtons();
        ClosePanel(confirmPanel);
    }
}