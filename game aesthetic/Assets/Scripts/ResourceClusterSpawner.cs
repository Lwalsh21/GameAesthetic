using UnityEngine;
using System.Collections.Generic;

public class ResourceClusterSpawner : MonoBehaviour
{
    [Header("Resource Prefabs")]
    public GameObject woodNodePrefab;
    public GameObject stoneNodePrefab;
    public GameObject manaNodePrefab;
    public GameObject foodNodePrefab;

    [Header("Cluster Settings (Wood, Stone, Food)")]
    public int woodClusters = 5;
    public int stoneClusters = 3;
    public int foodClusters = 4;

    public Vector2 woodClusterSize = new Vector2(8, 15);
    public Vector2 stoneClusterSize = new Vector2(4, 8);
    public Vector2 foodClusterSize = new Vector2(5, 10);

    [Header("Mana Settings (NO CLUSTERS)")]
    public int manaCount = 20;

    [Header("Spawn Area")]
    public float spawnRadius = 120f;
    public float minClusterSpacing = 20f;
    public float minNodeSpacing = 2f;

    private List<Vector3> clusterCenters = new List<Vector3>();

    private void Start()
    {
        Debug.Log("ResourceClusterSpawner: Start() running");
        // ⭐ Clustered resources
        SpawnClusters(woodNodePrefab, woodClusters, woodClusterSize);
        SpawnClusters(stoneNodePrefab, stoneClusters, stoneClusterSize);
        SpawnClusters(foodNodePrefab, foodClusters, foodClusterSize);

        // ⭐ Mana spawns individually, NOT clustered
        SpawnScatteredMana();
    }

    // -------------------------
    // CLUSTER SPAWNING
    // -------------------------
    private void SpawnClusters(GameObject prefab, int clusterCount, Vector2 clusterSizeRange)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[ResourceClusterSpawner] Missing prefab reference.");
            return;
        }

        for (int i = 0; i < clusterCount; i++)
        {
            Vector3 center = GetValidClusterCenter();
            clusterCenters.Add(center);

            int nodesInCluster = Random.Range((int)clusterSizeRange.x, (int)clusterSizeRange.y + 1);

            for (int j = 0; j < nodesInCluster; j++)
            {
                Vector3 offset = Random.insideUnitSphere * 6f;
                offset.y = 0;

                Vector3 pos = center + offset;

                // ⭐ Snap to terrain
                pos.y = Terrain.activeTerrain.SampleHeight(pos);

                // ⭐ Avoid overlapping nodes
                if (IsNodeTooClose(pos))
                {
                    j--;
                    continue;
                }

                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }

    private Vector3 GetValidClusterCenter()
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

            if (Vector3.Distance(transform.position, pos) > spawnRadius)
                continue;

            bool tooClose = false;
            foreach (var center in clusterCenters)
            {
                if (Vector3.Distance(center, pos) < minClusterSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return pos;

        } while (attempts < 50);

        return pos;
    }

    private bool IsNodeTooClose(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, minNodeSpacing);
        return hits.Length > 0;
    }

    // -------------------------
    // MANA SCATTER SPAWNING
    // -------------------------
    private void SpawnScatteredMana()
    {
        if (manaNodePrefab == null)
        {
            Debug.LogWarning("[ResourceClusterSpawner] Missing mana prefab reference.");
            return;
        }

        for (int i = 0; i < manaCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            // Keep inside circle
            if (Vector3.Distance(transform.position, pos) > spawnRadius)
            {
                i--;
                continue;
            }

            // Snap to terrain
            pos.y = Terrain.activeTerrain.SampleHeight(pos);

            // Avoid overlapping other nodes
            if (IsNodeTooClose(pos))
            {
                i--;
                continue;
            }

            Instantiate(manaNodePrefab, pos, Quaternion.identity);
        }
    }
}
