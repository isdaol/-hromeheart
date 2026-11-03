using UnityEngine;

/// <summary>
/// Progress shared by the level selector and the level completion trigger.
/// Add each new gameplay scene to SceneNames in progression order.
/// </summary>
public static class LevelProgress
{
    const string UnlockedLevelsKey = "UnlockedLevelCount";

    // Physics Game is the first playable level.
    static readonly string[] SceneNames = { "Physics Game", "Level2", "Level3" };

    public static int LevelCount => SceneNames.Length;

    public static string GetSceneName(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= SceneNames.Length)
            return null;

        return SceneNames[levelIndex];
    }

    public static string GetDisplayName(int levelIndex)
    {
        return "Level " + (levelIndex + 1);
    }

    public static bool IsUnlocked(int levelIndex)
    {
        return levelIndex >= 0 && levelIndex < SceneNames.Length &&
            levelIndex < PlayerPrefs.GetInt(UnlockedLevelsKey, 1);
    }

    public static void CompleteScene(string sceneName)
    {
        int completedIndex = System.Array.IndexOf(SceneNames, sceneName);
        if (completedIndex < 0)
            return;

        int unlockedCount = PlayerPrefs.GetInt(UnlockedLevelsKey, 1);
        int nextUnlockedCount = completedIndex + 2;
        if (nextUnlockedCount > unlockedCount)
        {
            PlayerPrefs.SetInt(UnlockedLevelsKey, nextUnlockedCount);
            PlayerPrefs.Save();
        }
    }
}
