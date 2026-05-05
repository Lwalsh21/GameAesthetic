using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    // Existing method (unchanged)
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;

        newObject.GetComponent<Constructable>().ConstructableWasPlaced();

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    // ⭐ NEW METHOD — lets PlacementState adjust the Y height
    public GameObject PlaceObjectAndReturn(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab);

        newObject.GetComponent<Constructable>().ConstructableWasPlaced();

        placedGameObjects.Add(newObject);

        return newObject;
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
