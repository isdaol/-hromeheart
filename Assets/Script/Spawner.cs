using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Scene Reference")]
    public SpriteRenderer background;

    [Header("Level Data")]
    [SerializeField] private LevelData levelData;

    private float timer;

    void Start()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned to Spawner.", this);
            enabled = false;
            return;
        }

        if (!levelData.useSpawner)
        {
            enabled = false;
            return;
        }

        if (background == null)
        {
            Debug.LogError("Background is not assigned to Spawner.", this);
            enabled = false;
            return;
        }

        if (levelData.hazards == null || levelData.hazards.Length == 0)
        {
            Debug.LogError("No hazard prefabs are assigned in LevelData.", this);
            enabled = false;
            return;
        }

        if (levelData.normalizedSpawnPoints == null ||
            levelData.normalizedSpawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points are assigned in LevelData.", this);
            enabled = false;
            return;
        }

        if (levelData.spawnInterval <= 0f)
        {
            Debug.LogError("Spawner interval must be greater than zero.", this);
            enabled = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= levelData.spawnInterval)
        {
            SpawnHazard();
            timer = 0f;
        }
    }

    void SpawnHazard()
    {
        Bounds backgroundBounds = background.bounds;

        float minX = backgroundBounds.min.x + levelData.sidePadding;
        float maxX = backgroundBounds.max.x - levelData.sidePadding;

        if (minX >= maxX)
        {
            Debug.LogWarning("Background is too narrow for spawning.", this);
            return;
        }

        Hazard selectedHazard =
            levelData.hazards[Random.Range(0, levelData.hazards.Length)];

        if (selectedHazard == null)
        {
            Debug.LogWarning("A hazard entry in LevelData is empty.", this);
            return;
        }

        Vector2 spawnPoint = levelData.normalizedSpawnPoints[
            Random.Range(0, levelData.normalizedSpawnPoints.Length)
        ];

        float normalizedX = Mathf.Clamp01(spawnPoint.x);
        float normalizedY = Mathf.Clamp01(spawnPoint.y);

        float spawnX = Mathf.Lerp(minX, maxX, normalizedX);
        float spawnY = Mathf.Lerp(
            backgroundBounds.min.y,
            backgroundBounds.max.y,
            normalizedY
        ) - levelData.topOffset;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        Instantiate(
            selectedHazard,
            spawnPosition,
            Quaternion.identity
        );
    }
}