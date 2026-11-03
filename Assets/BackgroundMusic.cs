using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource source;

    public static void EnsurePlaying(AudioClip clip)
    {
        if (instance != null)
            return;

        var musicObject = new GameObject("Background Music");
        var music = musicObject.AddComponent<BackgroundMusic>();
        music.Initialize(clip);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Initialize(AudioClip clip)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = PlayerPrefs.GetFloat("Volume", 1f);
        source.mute = PlayerPrefs.GetInt("Music", 1) == 0;

        if (clip != null)
            source.Play();
        else
            Debug.LogError("Background music clip is not assigned.");
    }

    public static void SetMusicEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("Music", enabled ? 1 : 0);
        if (instance != null)
            instance.source.mute = !enabled;
    }

    public static void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
        if (instance != null)
            instance.source.volume = volume;
    }
}
