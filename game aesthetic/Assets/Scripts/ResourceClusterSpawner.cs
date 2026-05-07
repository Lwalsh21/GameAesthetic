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
    private Terrain terrain;

    void Awake()
    {
        Debug.Log("<color=cyan>[Spawner]</color> Awake() running");

        terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            Debug.LogError("<color=red>[Spawner]</color> No active terrain found. Spawner DISABLED.");
            enabled = false;
        }
    }

    private void Start()
    {
        Debug.Log("<color=cyan>[Spawner]</color> Start() running");

        if (!enabled) return;

        SafeSpawnClusters("Wood", woodNodePrefab, woodClusters, woodClusterSize);
        SafeSpawnClusters("Stone", stoneNodePrefab, stoneClusters, stoneClusterSize);
        SafeSpawnClusters("Food", foodNodePrefab, foodClusters, foodClusterSize);

        SafeSpawnMana();
    }

    // ----------------------------------------------------------
    // SAFE CLUSTER SPAWNING
    // ----------------------------------------------------------
    private void SafeSpawnClusters(string label, GameObject prefab, int clusterCount, Vector2 clusterSizeRange)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"<color=yellow>[Spawner]</color> {label} prefab is NULL. Skipping.");
            return;
        }

        if (clusterCount <= 0)
        {
            Debug.LogWarning($"<color=yellow>[Spawner]</color> {label} cluster count is 0. Skipping.");
            return;
        }

        Debug.Log($"<color=green>[Spawner]</color> Spawning {clusterCount} {label} clusters...");

        for (int i = 0; i < clusterCount; i++)
        {
            if (!TryGetValidClusterCenter(out Vector3 center))
            {
                Debug.LogWarning($"<color=yellow>[Spawner]</color> Could not find valid center for {label} cluster {i}. Skipping.");
                continue;
            }

            clusterCenters.Add(center);

            int nodes = Random.Range((int)clusterSizeRange.x, (int)clusterSizeRange.y + 1);

            for (int j = 0; j < nodes; j++)
            {
                Vector3 pos = center + Random.insideUnitSphere * 6f;
                pos.y = terrain.SampleHeight(pos);

                if (IsNodeTooClose(pos))
                    continue;

                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }

    private bool TryGetValidClusterCenter(out Vector3 center)
    {
        center = Vector3.zero;

        for (int attempts = 0; attempts < 50; attempts++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            if (Vector3.Distance(transform.position, pos) > spawnRadius)
                continue;

            bool tooClose = false;
            foreach (var c in clusterCenters)
            {
                if (Vector3.Distance(c, pos) < minClusterSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                center = pos;
                return true;
            }
        }

        return false;
    }

    private bool IsNodeTooClose(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, minNodeSpacing);
        return hits.Length > 0;
    }

    // ----------------------------------------------------------
    // SAFE MANA SPAWNING
    // ----------------------------------------------------------
    private void SafeSpawnMana()
    {
        if (manaNodePrefab == null)
        {
            Debug.LogWarning("<color=yellow>[Spawner]</color> Mana prefab is NULL. Skipping.");
            return;
        }

        Debug.Log($"<color=green>[Spawner]</color> Spawning {manaCount} mana nodes...");

        for (int i = 0; i < manaCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            if (Vector3.Distance(transform.position, pos) > spawnRadius)
                continue;

            pos.y = terrain.SampleHeight(pos);

            if (IsNodeTooClose(pos))
                continue;

            Instantiate(manaNodePrefab, pos, Quaternion.identity);
        }
    }
}
