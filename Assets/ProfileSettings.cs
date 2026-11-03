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

    public void SaveName(string value)
    {
        PlayerPrefs.SetString("PlayerName", value);
    }

    public void SaveColor(int index)
    {
        PlayerPrefs.SetInt("PlayerColor", index);
    }

    public void SetFullscreen(bool isOn)
    {
        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
    }

    public void SetMusic(bool isOn)
    {
        BackgroundMusic.SetMusicEnabled(isOn);
    }

    public void SetVolume(float value)
    {
        BackgroundMusic.SetVolume(value);
    }

    void Start()
    {
        nameField.text = PlayerPrefs.GetString("PlayerName", "");

        colorDropdown.value = PlayerPrefs.GetInt("PlayerColor", 0);

        fullscreenToggle.isOn =
            PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        musicToggle.isOn =
            PlayerPrefs.GetInt("Music", 1) == 1;

        volumeSlider.value =
            PlayerPrefs.GetFloat("Volume", 1f);
    }
}
