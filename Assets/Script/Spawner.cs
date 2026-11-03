
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Background")]
    public SpriteRenderer background;
    public float sidePadding = 0.5f;
    public float topOffset = 0.5f;

    [Header("Hazard Prefabs")]
    public Hazard[] hazards;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;

    private float timer;

    void Update()
    {
        if (background == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnHazard();
            timer = 0f;
        }
    }

    void SpawnHazard()
    {
        if (hazards == null || hazards.Length == 0)
        {
            Debug.LogWarning("Assign Hazard prefabs in Spawner!");
            return;
        }

        Bounds bgBounds = background.bounds;

        // Ограничения по бокам фона
        float minX = bgBounds.min.x + sidePadding;
        float maxX = bgBounds.max.x - sidePadding;

        if (minX >= maxX)
        {
            Debug.LogWarning("Background is too narrow for spawning!");
            return;
        }

        int index = Random.Range(0, hazards.Length);
        Hazard selectedHazard = hazards[index];

        if (selectedHazard == null)
            return;

        // Высота появления относительно верхнего края фона
        float spawnY = bgBounds.max.y - topOffset;

        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPosition = new Vector3(
            randomX,
            spawnY,
            0f
        );

        Instantiate(
            selectedHazard,
            spawnPosition,
            Quaternion.identity
        );
    }
}
