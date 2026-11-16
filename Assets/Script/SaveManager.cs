using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const int CurrentVersion = 1;

    public static SaveManager Instance { get; private set; }

    public SaveData Data { get; private set; }

    public string SaveFilePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "save.json"
            );
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSave();
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            SaveNow();
    }

    void OnApplicationQuit()
    {
        SaveNow();
    }

    public void LoadSave()
    {
        if (!File.Exists(SaveFilePath))
        {
            Data = CreateDataFromOldPlayerPrefs();
            SaveNow();
            return;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

            if (loadedData == null ||
                loadedData.version != CurrentVersion)
            {
                Debug.LogWarning(
                    "Save file is empty or has an unsupported version. " +
                    "Loading default data."
                );

                Data = CreateDefaultData();
                SaveNow();
                return;
            }

            Data = loadedData;
            NormalizeData();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "Could not read save file. Loading default data. " +
                exception.Message
            );

            Data = CreateDefaultData();
            SaveNow();
        }
    }

    public void SaveNow()
    {
        if (Data == null)
            return;

        try
        {
            string directory = Path.GetDirectoryName(SaveFilePath);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(SaveFilePath, json);

            Debug.Log("Save written to: " + SaveFilePath);
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Could not write save file: " + exception.Message
            );
        }
    }

    public void ResetProgress()
    {
        if (Data == null)
            Data = CreateDefaultData();

        Data.bestScore = 0;
        Data.unlockedLevelCount = 1;

        SaveNow();
    }

    SaveData CreateDefaultData()
    {
        return new SaveData
        {
            version = CurrentVersion,
            bestScore = 0,
            unlockedLevelCount = 1,
            playerName = "",
            playerColor = 0,
            fullscreen = true,
            musicEnabled = true,
            musicVolume = 1f
        };
    }

    SaveData CreateDataFromOldPlayerPrefs()
    {
        return new SaveData
        {
            version = CurrentVersion,
            bestScore = 0,

            unlockedLevelCount = PlayerPrefs.GetInt(
                "UnlockedLevelCount",
                1
            ),

            playerName = PlayerPrefs.GetString(
                "PlayerName",
                ""
            ),

            playerColor = PlayerPrefs.GetInt(
                "PlayerColor",
                0
            ),

            fullscreen = PlayerPrefs.GetInt(
                "Fullscreen",
                1
            ) == 1,

            musicEnabled = PlayerPrefs.GetInt(
                "Music",
                1
            ) == 1,

            musicVolume = PlayerPrefs.GetFloat(
                "Volume",
                1f
            )
        };
    }

    void NormalizeData()
    {
        if (Data.playerName == null)
            Data.playerName = "";

        Data.bestScore = Mathf.Max(0, Data.bestScore);
        Data.unlockedLevelCount = Mathf.Max(1, Data.unlockedLevelCount);
        Data.playerColor = Mathf.Clamp(Data.playerColor, 0, 2);
        Data.musicVolume = Mathf.Clamp01(Data.musicVolume);
    }
}