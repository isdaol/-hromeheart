using System;

[Serializable]
public class SaveData
{
    public int version;
    public int bestScore;
    public int unlockedLevelCount;

    public string playerName;
    public int playerColor;
    public bool fullscreen;
    public bool musicEnabled;
    public float musicVolume;
    public float sfxVolume = 1f;
}
