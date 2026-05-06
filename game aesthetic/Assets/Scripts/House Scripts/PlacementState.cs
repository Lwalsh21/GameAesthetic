using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;

    private readonly int id;
    private readonly Grid grid;
    private readonly PreviewSystem previewSystem;
    private readonly ObjectsDatabseSO database;
    private readonly GridData floorData;
    private readonly GridData furnitureData;
    private readonly ObjectPlacer objectPlacer;

    public PlacementState(
        int id,
        Grid grid,
        PreviewSystem previewSystem,
        ObjectsDatabseSO database,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objectPlacer)
    {
        this.id = id;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(d => d.ID == id);

        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"[PlacementState] No ObjectData found for ID {id}");
            return;
        }

        ObjectData data = database.objectsData[selectedObjectIndex];

        if (data == null)
        {
            Debug.LogError($"[PlacementState] ObjectData at index {selectedObjectIndex} is NULL for ID {id}");
            return;
        }

        if (data.Prefab == null)
        {
            Debug.LogError($"[PlacementState] Object '{data.Name}' (ID {id}) has NO prefab assigned!");
            return;
        }

        previewSystem.StartShowingPlacementPreview(data.Prefab, data.Size);
        Debug.Log($"[PlacementState] Started placement for '{data.Name}' (ID {id}, index {selectedObjectIndex})");
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
        CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
    }

    public bool IsFloor(int id)
    {
        return id == 11; // or whatever your floor IDs are
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (selectedObjectIndex < 0)
            return;

        ObjectData data = database.objectsData[selectedObjectIndex];

        // ⭐ Dependency check
        if (!DependencyManager.Instance.AreRequirementsMet(data))
        {
            Debug.Log($"[PlacementState] Cannot place {data.Name}. Requirements not met.");
            return;
        }

        // ⭐ Placement rule check (only if restricted)
        if (data.restrictPlacement)
        {
            if (!CheckPlacementValidity(gridPosition, selectedObjectIndex))
            {
                Debug.Log("[PlacementState] Invalid placement position.");
                return;
            }
        }

        // Convert grid cell to world position
        Vector3 basePos = grid.CellToWorld(gridPosition);
        basePos.y = 1f; // force height

        // Instantiate the object
        GameObject placedObj = objectPlacer.PlaceObjectAndReturn(data.Prefab);
        placedObj.transform.position = basePos;

        GridData gridToWrite = data.restrictPlacement
        ? (GetAllFloorIDs().Contains(data.ID) ? floorData : furnitureData)
        : null;

        BuildingHealth health = placedObj.AddComponent<BuildingHealth>();
        health.Initialize(data.ID, gridPosition, data.Size, gridToWrite);

        // ⭐ Register in grid ONLY if restricted
        if (data.restrictPlacement)
        {
            GridData selectedData = GetAllFloorIDs().Contains(data.ID) ? floorData : furnitureData;

            selectedData.AddObjectAt(
                gridPosition,
                data.Size,
                data.ID,
                placedObj.GetInstanceID());
        }

        // Remove resources
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.RemoveResourcesBasedOnRequirement(data);
        else
            Debug.LogWarning("[PlacementState] ResourceManager.Instance is null, cannot remove resources.");

        // ⭐ Register building for dependency unlocking
        DependencyManager.Instance.RegisterBuilding(data.ID);

        // ⭐ Refresh UI buttons
        ConstructionSlot[] slots = GameObject.FindObjectsOfType<ConstructionSlot>(true);
        foreach (var slot in slots)
            slot.SendMessage("HandleResourceChange", SendMessageOptions.DontRequireReceiver);

        // Update preview
        previewSystem.UpdatePosition(basePos, false);
    }

    private List<int> GetAllFloorIDs()
    {
        return new List<int> { 11 };
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        if (selectedObjectIndex < 0)
            return false;

        ObjectData data = database.objectsData[selectedObjectIndex];

        // ⭐ If unrestricted, always valid
        if (!data.restrictPlacement)
            return true;

        GridData selectedData = GetAllFloorIDs().Contains(data.ID) ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, data.Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (selectedObjectIndex < 0)
            return;

        ObjectData data = database.objectsData[selectedObjectIndex];

        bool depsMet = DependencyManager.Instance.AreRequirementsMet(data);
        bool placementValidity;

        if (!data.restrictPlacement)
        {
            // ⭐ Always valid if unrestricted
            placementValidity = depsMet;
        }
        else
        {
            placementValidity = depsMet &&
                                CheckPlacementValidity(gridPosition, selectedObjectIndex);
        }

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);

        CursorManager.Instance.SetMarkerType(
            placementValidity ? CursorManager.CursorType.Walkable
                              : CursorManager.CursorType.Unavailable
        );
    }
}
