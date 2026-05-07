using UnityEngine;
using System.Collections.Generic;

public class ResourceClusterSpawner : MonoBehaviour
{
    [Header("Resource Prefabs")]
    public GameObject woodNodePrefab;
    public GameObject stoneNodePrefab;
    public GameObject manaNodePrefab;
    public GameObject foodNodePrefab;

    [Header("Terrain Reference (Assign in Inspector)")]
    public Terrain terrain;

    [Header("Collision Layers")]
    public LayerMask blockingLayers;      // Wood/Stone/Food
    public LayerMask manaBlockingLayers;  // Mana wells only

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

    void Awake()
    {
        if (terrain == null)
        {
            Debug.LogError("<color=red>[Spawner]</color> No terrain assigned. DISABLED.");
            enabled = false;
        }
    }

    private void Start()
    {
        if (!enabled) return;

        SafeSpawnClusters("Wood", woodNodePrefab, woodClusters, woodClusterSize);
        SafeSpawnClusters("Stone", stoneNodePrefab, stoneClusters, stoneClusterSize);
        SafeSpawnClusters("Food", foodNodePrefab, foodClusters, foodClusterSize);

        SafeSpawnMana();
    }

    // ----------------------------------------------------------
    // TERRAIN BOUNDS CHECK
    // ----------------------------------------------------------
    private bool IsInsideTerrain(Vector3 pos)
    {
        Vector3 tPos = terrain.transform.position;
        Vector3 tSize = terrain.terrainData.size;

        return pos.x >= tPos.x &&
               pos.x <= tPos.x + tSize.x &&
               pos.z >= tPos.z &&
               pos.z <= tPos.z + tSize.z;
    }

    // ----------------------------------------------------------
    // SPACING CHECKS
    // ----------------------------------------------------------
    private bool IsNodeTooClose(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(
            pos,
            minNodeSpacing,
            blockingLayers,
            QueryTriggerInteraction.Ignore
        );

        return hits.Length > 0;
    }

    private bool IsManaTooClose(Vector3 pos)
    {
        // Mana wells only avoid overlapping other mana wells
        Collider[] hits = Physics.OverlapSphere(
            pos,
            1.0f, // small radius so they can be close together
            manaBlockingLayers,
            QueryTriggerInteraction.Ignore
        );

        return hits.Length > 0;
    }

    // ----------------------------------------------------------
    // SAFE CLUSTER SPAWNING
    // ----------------------------------------------------------
    private void SafeSpawnClusters(string label, GameObject prefab, int clusterCount, Vector2 clusterSizeRange)
    {
        if (prefab == null || clusterCount <= 0)
            return;

        for (int i = 0; i < clusterCount; i++)
        {
            if (!TryGetValidClusterCenter(out Vector3 center))
                continue;

            clusterCenters.Add(center);

            int nodes = Random.Range((int)clusterSizeRange.x, (int)clusterSizeRange.y + 1);

            for (int j = 0; j < nodes; j++)
            {
                Vector3 pos = center + Random.insideUnitSphere * 6f;
                pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y;

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

            if (!IsInsideTerrain(pos))
                continue;

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

    // ----------------------------------------------------------
    // SAFE MANA SPAWNING
    // ----------------------------------------------------------
    private void SafeSpawnMana()
    {
        if (manaNodePrefab == null)
            return;

        for (int i = 0; i < manaCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            if (!IsInsideTerrain(pos))
                continue;

            if (Vector3.Distance(transform.position, pos) > spawnRadius)
                continue;

            // Mana spawns at pivot height (not terrain height)
            pos.y = transform.position.y;

            // Mana only avoids overlapping other mana wells
            if (IsManaTooClose(pos))
                continue;

            Instantiate(manaNodePrefab, pos, Quaternion.identity);
        }
    }
}
