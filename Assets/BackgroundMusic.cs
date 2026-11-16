using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource source;

    public static void EnsurePlaying(AudioClip clip)
    {
        if (instance != null)
            return;

        GameObject musicObject = new GameObject("Background Music");
        BackgroundMusic music =
            musicObject.AddComponent<BackgroundMusic>();

        music.Initialize(clip);
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Initialize(AudioClip clip)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;

        SaveManager saveManager = SaveManager.Instance;

        if (saveManager != null && saveManager.Data != null)
        {
            source.volume = Mathf.Clamp01(
                saveManager.Data.musicVolume
            );
            source.mute = !saveManager.Data.musicEnabled;
        }
        else
        {
            source.volume = 1f;
            source.mute = false;
        }

        if (clip != null)
            source.Play();
        else
            Debug.LogError("Background music clip is not assigned.");
    }

    public static void ApplySettings(bool musicEnabled, float volume)
    {
        if (instance == null || instance.source == null)
            return;

        instance.source.mute = !musicEnabled;
        instance.source.volume = Mathf.Clamp01(volume);
    }

    public static void SetMusicEnabled(bool isEnabled)
    {
        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager data is not available.");
            return;
        }

        saveManager.Data.musicEnabled = isEnabled;
        saveManager.SaveNow();

        if (instance != null && instance.source != null)
            instance.source.mute = !isEnabled;
    }

    public static void SetVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager data is not available.");
            return;
        }

        saveManager.Data.musicVolume = clampedVolume;
        saveManager.SaveNow();

        if (instance != null && instance.source != null)
            instance.source.volume = clampedVolume;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}