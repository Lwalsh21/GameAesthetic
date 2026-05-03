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

    public void OnAction(Vector3Int gridPosition)
    {
        if (selectedObjectIndex < 0)
            return;

        ObjectData data = database.objectsData[selectedObjectIndex];

        if (!CheckPlacementValidity(gridPosition, selectedObjectIndex))
        {
            Debug.Log("[PlacementState] Invalid placement position.");
            return;
        }

        int placedIndex = objectPlacer.PlaceObject(
            data.Prefab,
            grid.CellToWorld(gridPosition));

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.RemoveResourcesBasedOnRequirement(data);
        }
        else
        {
            Debug.LogWarning("[PlacementState] ResourceManager.Instance is null, cannot remove resources.");
        }

        GridData selectedData = GetAllFloorIDs().Contains(data.ID) ? floorData : furnitureData;

        selectedData.AddObjectAt(
            gridPosition,
            data.Size,
            data.ID,
            placedIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private List<int> GetAllFloorIDs()
    {
        // Add all IDs that should be treated as floor here
        return new List<int> { 11 };
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        if (selectedObjectIndex < 0)
            return false;

        ObjectData data = database.objectsData[selectedObjectIndex];
        GridData selectedData = GetAllFloorIDs().Contains(data.ID) ? floorData : furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, data.Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (selectedObjectIndex < 0)
            return;

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);

        // NEW: Update cursor
        if (placementValidity)
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable);
        else
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Unavailable);
    }
}
