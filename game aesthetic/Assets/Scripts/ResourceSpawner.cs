using UnityEngine;
using System.Collections.Generic;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Resource Prefabs")]
    public GameObject woodNodePrefab;
    public GameObject stoneNodePrefab;
    public GameObject manaNodePrefab;
    public GameObject foodNodePrefab;

    [Header("Spawn Settings")]
    public int woodCount = 50;
    public int stoneCount = 30;
    public int manaCount = 20;
    public int foodCount = 40;

    public float spawnRadius = 100f;
    public float minSpacing = 3f;

    private List<Vector3> usedPositions = new List<Vector3>();

    private void Start()
    {
        SpawnNodes(woodNodePrefab, woodCount);
        SpawnNodes(stoneNodePrefab, stoneCount);
        SpawnNodes(manaNodePrefab, manaCount);
        SpawnNodes(foodNodePrefab, foodCount);
    }

    private void SpawnNodes(GameObject prefab, int count)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[ResourceSpawner] Missing prefab reference.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetValidSpawnPosition();

            // ⭐ Snap to terrain height
            pos.y = Terrain.activeTerrain.SampleHeight(pos);

            Instantiate(prefab, pos, Quaternion.identity);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 pos;
        int attempts = 0;

        do
        {
            attempts++;
            pos = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            // ⭐ Ensure position is within circle, not square
            if (Vector3.Distance(transform.position, pos) > spawnRadius)
                continue;

            // ⭐ Ensure spacing between nodes
            bool tooClose = false;
            foreach (var used in usedPositions)
            {
                if (Vector3.Distance(used, pos) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                usedPositions.Add(pos);
                return pos;
            }

        } while (attempts < 50);

        // fallback
        return pos;
    }
}
