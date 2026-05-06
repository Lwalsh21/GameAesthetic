using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    // ⭐ Helper: Get mesh bottom offset so buildings sit correctly on terrain
    private float GetMeshBottomOffset(GameObject obj)
    {
        MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
        if (renderer == null)
            return 0f;

        return -renderer.bounds.min.y;
    }

    // ⭐ OLD METHOD (kept for compatibility)
    // Now terrain-aware + mesh-offset-aware
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        // Snap to terrain
        position.y = Terrain.activeTerrain.SampleHeight(position);

        // Instantiate
        GameObject newObject = Instantiate(prefab);

        // Apply mesh offset
        float offset = GetMeshBottomOffset(newObject);
        position.y += offset;

        newObject.transform.position = position;

        // Constructable logic
        Constructable constructable = newObject.GetComponent<Constructable>();
        if (constructable != null)
            constructable.ConstructableWasPlaced();

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    // ⭐ NEW METHOD — used by PlacementState
    // Now accepts a world position
    public GameObject PlaceObjectAndReturn(GameObject prefab, Vector3 position)
    {
        // Snap to terrain
        position.y = Terrain.activeTerrain.SampleHeight(position);

        // Instantiate
        GameObject newObject = Instantiate(prefab);

        // Apply mesh offset
        float offset = GetMeshBottomOffset(newObject);
        position.y += offset;

        newObject.transform.position = position;

        // Constructable logic
        Constructable constructable = newObject.GetComponent<Constructable>();
        if (constructable != null)
            constructable.ConstructableWasPlaced();

        placedGameObjects.Add(newObject);

        return newObject;
    }

    // ⭐ Backwards compatibility: if PlacementState calls the old version
    public GameObject PlaceObjectAndReturn(GameObject prefab)
    {
        // Place at prefab's current position (rarely used)
        Vector3 pos = prefab.transform.position;
        return PlaceObjectAndReturn(prefab, pos);
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null)
            return;

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
