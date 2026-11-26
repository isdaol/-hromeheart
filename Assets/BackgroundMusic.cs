using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private const string MenuScene = "Main menu";
    private const float FadeSeconds = 1f;
    private AudioSource menuTrack;
    private AudioSource levelTrack;
    private float levelMix;
    private float volume = 1f;
    private bool musicEnabled = true;
    private bool playingLevel;
    private bool mixerControlsVolume;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartInGameplayScene()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
            EnsurePlaying(null);
    }

    public static void EnsurePlaying(AudioClip menuClip)
    {
        if (instance != null)
        {
            if (menuClip != null && instance.menuTrack.clip == null)
                instance.menuTrack.clip = menuClip;

            return;
        }

        GameObject musicObject = new GameObject("Background Music");
        BackgroundMusic music =
            musicObject.AddComponent<BackgroundMusic>();

        music.Initialize(menuClip);
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ApplySettings(musicEnabled, volume);
    }

    private void Initialize(AudioClip menuClip)
    {
        menuTrack = CreateTrack(menuClip);
        levelTrack = CreateTrack(GameSounds.LoadClip("specular-city"));

        SaveData save = SaveManager.Instance != null ? SaveManager.Instance.Data : null;
        if (save != null)
        {
            volume = Mathf.Clamp01(save.musicVolume);
            musicEnabled = save.musicEnabled;
        }

        playingLevel = menuClip == null;
        levelMix = playingLevel ? 1f : 0f;
        ApplyMix();
        StartTrack(playingLevel ? levelTrack : menuTrack);
    }

    private AudioSource CreateTrack(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = GameSounds.MusicOutput;
        return source;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single)
            return;

        bool isLevel = scene.name != MenuScene;
        AudioSource nextTrack = isLevel ? levelTrack : menuTrack;
        if (nextTrack.clip == null)
            return;

        playingLevel = isLevel;
        StartTrack(nextTrack);
    }

    private static void StartTrack(AudioSource source)
    {
        if (source.clip != null && !source.isPlaying)
            source.Play();
    }

    private void Update()
    {
        float target = playingLevel ? 1f : 0f;
        if (levelMix == target)
            return;

        levelMix = Mathf.MoveTowards(levelMix, target, Time.unscaledDeltaTime / FadeSeconds);
        ApplyMix();

        if (levelMix == target)
            (playingLevel ? menuTrack : levelTrack).Stop();
    }

    private void ApplyMix()
    {
        float trackGain = mixerControlsVolume ? 1f : volume;
        menuTrack.volume = trackGain * (1f - levelMix);
        levelTrack.volume = trackGain * levelMix;
        menuTrack.mute = !musicEnabled || volume == 0f;
        levelTrack.mute = !musicEnabled || volume == 0f;
    }

    public static void ApplySettings(bool musicEnabled, float volume)
    {
        if (instance == null)
            return;

        instance.musicEnabled = musicEnabled;
        instance.volume = Mathf.Clamp01(volume);
        instance.mixerControlsVolume = GameSounds.ApplyMusicVolume(instance.volume);
        instance.ApplyMix();
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

        ApplySettings(isEnabled, saveManager.Data.musicVolume);
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

        ApplySettings(saveManager.Data.musicEnabled, clampedVolume);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (instance == this)
            instance = null;
    }
}
