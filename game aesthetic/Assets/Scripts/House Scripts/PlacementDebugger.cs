using UnityEngine;

public class PlacementDebugger : MonoBehaviour
{
    public InputManager input;
    public PlacementSystem placement;
    public PreviewSystem preview;

    void Update()
    {
        Debug.Log(
            "UI: " + input.IsPointerOverUI() +
            " | MousePos: " + Input.mousePosition +
            " | WorldPos: " + input.GetSelectedMapPosition() +
            " | BuildingState: " + (placement != null ? "Active" : "NULL") +
            " | PreviewObject: " + (preview != null ? "Exists" : "NULL")
        );
    }
}
