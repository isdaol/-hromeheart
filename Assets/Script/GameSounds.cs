using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameSounds : MonoBehaviour
{
    private const string ResourceFolder = "SoundPass/";
    private const float DeathCueTime = 0.2f;
    private const float DeathFadeTime = 0.12f;
    private const string MusicVolumeParameter = "MusicVolume";
    private const string SfxVolumeParameter = "SfxVolume";
    private const float PauseTransitionTime = 0.2f;
    private const float MinimumEffectPitch = 0.92f;
    private const float MaximumEffectPitch = 1.08f;

    private static GameSounds instance;

    private AudioMixer mixer;
    private AudioMixerGroup musicGroup;
    private AudioMixerGroup effectsGroup;
    private AudioMixerSnapshot normalSnapshot;
    private AudioMixerSnapshot pausedSnapshot;
    private readonly List<AudioSource> gameplaySources = new List<AudioSource>();
    private readonly List<AudioSource> buttonSources = new List<AudioSource>();
    private AudioSource resultSource;
    private AudioClip jump;
    private AudioClip button;
    private AudioClip chargeWarning;
    private AudioClip shotWarning;
    private AudioClip death;
    private AudioClip victory;
    private AudioClip defeat;
    private string defeatedScene;
    private bool victoryPlayed;
    private bool musicPaused;

    internal static AudioMixerGroup MusicOutput => instance != null ? instance.musicGroup : null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        if (instance == null)
            new GameObject("Game Sounds").AddComponent<GameSounds>();
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        mixer = Resources.Load<AudioMixer>("ChromeheartAudio");
        if (mixer != null)
        {
            mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
            normalSnapshot = mixer.FindSnapshot("Normal");
            pausedSnapshot = mixer.FindSnapshot("Paused");
        }

        musicGroup = FindGroup("Master/Music/MusicPause");
        if (musicGroup == null)
            musicGroup = FindGroup("Master/Music");

        effectsGroup = FindGroup("Master/SFX");

        resultSource = CreateSource(gameObject);
        jump = LoadClip("jump");
        button = LoadClip("button");
        chargeWarning = LoadClip("dashcharge");
        shotWarning = LoadClip("lightningcharge");
        death = LoadClip("miss");
        victory = LoadClip("stage-clear-short");
        defeat = LoadClip("game-over");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null && saveManager.Data != null)
            ApplyVolume(SfxVolumeParameter, saveManager.Data.sfxVolume);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (instance == this)
            instance = null;
    }

    internal static AudioClip LoadClip(string name)
    {
        return Resources.Load<AudioClip>(ResourceFolder + name);
    }

    private AudioMixerGroup FindGroup(string path)
    {
        if (mixer == null)
            return null;

        AudioMixerGroup[] groups = mixer.FindMatchingGroups(path);
        return groups.Length > 0 ? groups[0] : null;
    }

    private bool ApplyVolume(string parameter, float volume)
    {
        float decibels = 20f * Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f));
        return mixer != null && mixer.SetFloat(parameter, decibels);
    }

    internal static bool ApplyMusicVolume(float volume)
    {
        return instance != null && instance.musicGroup != null &&
               instance.ApplyVolume(MusicVolumeParameter, volume);
    }

    public static void SetSfxVolume(float volume)
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager data is not available.");
            return;
        }

        volume = Mathf.Clamp01(volume);
        if (instance != null)
            instance.ApplyVolume(SfxVolumeParameter, volume);

        saveManager.Data.sfxVolume = volume;
        saveManager.SaveNow();
    }

    internal static void SetPaused(bool paused)
    {
        if (instance == null || instance.musicPaused == paused)
            return;

        AudioMixerSnapshot snapshot = paused ? instance.pausedSnapshot : instance.normalSnapshot;
        if (snapshot == null)
            return;

        instance.musicPaused = paused;
        snapshot.TransitionTo(PauseTransitionTime);
    }

    private AudioSource CreateSource(GameObject owner)
    {
        AudioSource source = owner.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = effectsGroup;
        return source;
    }

    internal static void PlayEffectAt(AudioClip clip, Vector3 position, float volume)
    {
        instance?.PlayGameplay(clip, volume, position);
    }

    internal static void PlayGameplayEffect(AudioClip clip)
    {
        instance?.PlayGameplay(clip, 1f);
    }

    private AudioSource GetAvailableSource(List<AudioSource> sources)
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
                return source;
        }

        GameObject sound = new GameObject("SFX Voice");
        sound.transform.SetParent(transform, false);
        AudioSource newSource = CreateSource(sound);
        sources.Add(newSource);
        return newSource;
    }

    private void PlayVariant(List<AudioSource> sources, AudioClip clip, float volume, Vector3? position = null)
    {
        AudioSource source = GetAvailableSource(sources);
        source.transform.position = position ?? transform.position;
        source.spatialBlend = position.HasValue ? 1f : 0f;
        source.clip = clip;
        source.volume = volume;
        source.pitch = Random.Range(MinimumEffectPitch, MaximumEffectPitch);
        source.Play();
    }

    private void StopGameplaySounds()
    {
        foreach (AudioSource source in gameplaySources)
            source.Stop();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
        {
            SetPaused(false);
            StopAllCoroutines();
            StopGameplaySounds();
            resultSource.Stop();
            resultSource.volume = 1f;

            if (defeatedScene != null)
                Time.timeScale = 1f;

            if (scene.name == defeatedScene && defeat != null)
                resultSource.PlayOneShot(defeat, 0.72f);

            defeatedScene = null;
            victoryPlayed = false;
        }

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Button sceneButton in root.GetComponentsInChildren<Button>(true))
                sceneButton.onClick.AddListener(PlayButton);
        }
    }

    private void PlayButton()
    {
        if (button != null)
            PlayVariant(buttonSources, button, 0.8f);
    }

    private void PlayGameplay(AudioClip clip, float volume, Vector3? position = null)
    {
        if (clip != null && defeatedScene == null && !victoryPlayed)
            PlayVariant(gameplaySources, clip, volume, position);
    }

    public static void PlayJump()
    {
        instance?.PlayGameplay(instance.jump, 0.55f);
    }

    public static void WarnCharge()
    {
        instance?.PlayGameplay(instance.chargeWarning, 0.8f);
    }

    public static void WarnShot()
    {
        instance?.PlayGameplay(instance.shotWarning, 0.8f);
    }

    public static void PlayVictory()
    {
        if (instance == null || instance.victoryPlayed || instance.defeatedScene != null)
            return;

        instance.victoryPlayed = true;
        instance.StopGameplaySounds();
        instance.resultSource.Stop();

        if (instance.victory != null)
            instance.resultSource.PlayOneShot(instance.victory, 0.8f);
    }

    public static void RestartAfterDefeat()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (instance.defeatedScene != null || instance.victoryPlayed)
            return;

        instance.defeatedScene = sceneName;
        instance.StartCoroutine(instance.PlayDeathAndRestart(sceneName));
    }

    private IEnumerator PlayDeathAndRestart(string sceneName)
    {
        Time.timeScale = 0f;
        StopGameplaySounds();
        resultSource.Stop();

        if (death != null)
        {
            const float volume = 0.8f;
            float duration = Mathf.Min(DeathCueTime, death.length);
            float fadeStarts = Mathf.Max(0f, duration - DeathFadeTime);
            resultSource.volume = volume;
            resultSource.PlayOneShot(death);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
                resultSource.volume = volume * (1f - Mathf.InverseLerp(fadeStarts, duration, elapsed));
            }

            resultSource.Stop();
            resultSource.volume = 1f;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
