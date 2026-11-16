using System;
using UnityEngine;

public static class LevelProgress
{
    private static LevelData[] levels;

    public static void Configure(LevelData[] levelData)
    {
        levels = levelData;
    }

    public static int LevelCount
    {
        get
        {
            return levels == null ? 0 : levels.Length;
        }
    }

    public static LevelData GetLevelData(int levelIndex)
    {
        if (levels == null ||
            levelIndex < 0 ||
            levelIndex >= levels.Length)
        {
            return null;
        }

        return levels[levelIndex];
    }

    public static string GetSceneName(int levelIndex)
    {
        LevelData level = GetLevelData(levelIndex);
        return level != null ? level.sceneName : null;
    }

    public static string GetDisplayName(int levelIndex)
    {
        LevelData level = GetLevelData(levelIndex);

        if (level != null && !string.IsNullOrEmpty(level.displayName))
            return level.displayName;

        return "Level " + (levelIndex + 1);
    }

    public static bool IsUnlocked(int levelIndex)
    {
        LevelData level = GetLevelData(levelIndex);
        SaveManager saveManager = SaveManager.Instance;

        if (level == null ||
            saveManager == null ||
            saveManager.Data == null)
        {
            return false;
        }

        return levelIndex < saveManager.Data.unlockedLevelCount;
    }

    public static void CompleteScene(string sceneName)
    {
        if (levels == null || string.IsNullOrEmpty(sceneName))
            return;

        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || saveManager.Data == null)
        {
            Debug.LogError("SaveManager is not available.");
            return;
        }

        int completedIndex = Array.FindIndex(
            levels,
            level => level != null && level.sceneName == sceneName
        );

        if (completedIndex < 0)
            return;

        int nextUnlockedCount = Mathf.Min(
            completedIndex + 2,
            levels.Length
        );

        if (nextUnlockedCount > saveManager.Data.unlockedLevelCount)
        {
            saveManager.Data.unlockedLevelCount = nextUnlockedCount;
        }

        Player2D player =
            UnityEngine.Object.FindAnyObjectByType<Player2D>();

        if (player != null)
        {
            saveManager.Data.bestScore = Mathf.Max(
                saveManager.Data.bestScore,
                player.CurrentScore
            );
        }

        saveManager.SaveNow();
    }
}