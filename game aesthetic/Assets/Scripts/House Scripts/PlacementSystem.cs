using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabseSO database;
    [SerializeField] private GridData floorData, furnitureData;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private int selectedID;
    private IBuildingState buildingState;

    private void Start()
    {
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int id)
    {
        Debug.Log($"[PlacementSystem] StartPlacement called with ID {id}");

        selectedID = id;

        StopPlacement();

        buildingState = new PlacementState(id, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();

        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            Debug.Log("[PlacementSystem] Pointer over UI, ignoring click.");
            return;
        }

        if (buildingState == null)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);

        ObjectData ob = database.GetObjectByID(selectedID);
        if (ob == null)
        {
            Debug.LogError($"[PlacementSystem] No ObjectData found for ID {selectedID} when applying benefits.");
            StopPlacement();
            return;
        }

        if (ob.benefits != null)
        {
            foreach (BuildBenefits bf in ob.benefits)
                CalculateAndAddBenefit(bf);
        }

        StopPlacement();
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
                // StatusManager.Instance.IncreaseHousing(bf.benefitAmount);
                break;
        }
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
        {
            Debug.Log("BUILDING STATE IS NULL");
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
