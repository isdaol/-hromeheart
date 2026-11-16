using UnityEngine;

public enum LevelGoalType
{
    CollectAllCoins,
    SurviveTimer
}

[CreateAssetMenu(
    fileName = "LevelData",
    menuName = "Chromeheart/Data/Level Data"
)]
public class LevelData : ScriptableObject
{
    [Header("Level")]
    public string displayName;
    public string sceneName;

    [Header("Goal")]
    public LevelGoalType goalType = LevelGoalType.CollectAllCoins;

    [Header("Timer")]
    [Min(0f)] public float survivalTime;

    [Header("Hazard Spawner")]
    public bool useSpawner;
    public Hazard[] hazards;
    public float spawnInterval;
    public float sidePadding;
    public float topOffset;

    [Tooltip("Позиции появления в пределах фона: X и Y от 0 до 1.")]
    public Vector2[] normalizedSpawnPoints;
}