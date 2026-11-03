using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject levelsPanel;

    public AudioSource bgSound;
    public AudioClip bgMusicClip;

    void Awake()
    {
        if (bgSound != null)
            bgSound.playOnAwake = false;

        BackgroundMusic.EnsurePlaying(bgMusicClip != null ? bgMusicClip : (bgSound != null ? bgSound.clip : null));
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (PlayerPrefs.GetInt("OpenLevelsOnMenuStart", 0) == 1)
        {
            PlayerPrefs.DeleteKey("OpenLevelsOnMenuStart");
            OpenLevels();
        }
    }

    public void Play()
    {
        OpenLevels();
    }

    public void OpenLevels()
    {
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
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting the game");
    }

    public void Settings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void Help()
    {
        mainPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    public void BackFromSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void BackFromHelp()
    {
        helpPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void SetVolume(float value)
    {
        BackgroundMusic.SetVolume(value);
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

}
