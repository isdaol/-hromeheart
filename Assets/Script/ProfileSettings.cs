using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileSettings : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_Dropdown colorDropdown;

    public Toggle fullscreenToggle;
    public Toggle musicToggle;

    public AudioSource bgSound;
    public Slider volumeSlider;
    public Slider sfxVolumeSlider;

    public void SaveName(string value)
    {
        SaveManager saveManager = GetSaveManager();

        if (saveManager == null)
            return;

        saveManager.Data.playerName = value ?? "";
        saveManager.SaveNow();
    }

    public void SaveColor(int index)
    {
        SaveManager saveManager = GetSaveManager();

        if (saveManager == null)
            return;

        if (colorDropdown != null && colorDropdown.options.Count > 0)
            index = Mathf.Clamp(index, 0, colorDropdown.options.Count - 1);

        saveManager.Data.playerColor = index;
        saveManager.SaveNow();
    }

    public void SetFullscreen(bool isOn)
    {
        Screen.fullScreen = isOn;

        SaveManager saveManager = GetSaveManager();

        if (saveManager == null)
            return;

        saveManager.Data.fullscreen = isOn;
        saveManager.SaveNow();
    }

    public void SetMusic(bool isOn)
    {
        BackgroundMusic.SetMusicEnabled(isOn);
    }

    public void SetVolume(float value)
    {
        BackgroundMusic.SetVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        GameSounds.SetSfxVolume(value);
    }

    void Start()
    {
        SaveManager saveManager = GetSaveManager();

        if (saveManager == null)
            return;

        SaveData data = saveManager.Data;

        if (nameField != null)
            nameField.SetTextWithoutNotify(data.playerName ?? "");

        if (colorDropdown != null && colorDropdown.options.Count > 0)
        {
            int colorIndex = Mathf.Clamp(
                data.playerColor,
                0,
                colorDropdown.options.Count - 1
            );

            colorDropdown.SetValueWithoutNotify(colorIndex);
        }

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(data.fullscreen);

        if (musicToggle != null)
            musicToggle.SetIsOnWithoutNotify(data.musicEnabled);

        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(data.musicVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(data.sfxVolume);

        Screen.fullScreen = data.fullscreen;

        BackgroundMusic.ApplySettings(
            data.musicEnabled,
            data.musicVolume
        );
    }

    SaveManager GetSaveManager()
    {
        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager data is not available.", this);
            return null;
        }

        return saveManager;
    }
}
