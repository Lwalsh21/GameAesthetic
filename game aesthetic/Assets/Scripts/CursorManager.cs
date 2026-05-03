using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Marker Prefabs")]
    public GameObject walkableCursor;
    public GameObject selectableCursor;
    public GameObject attackableCursor;
    public GameObject unavailableCursor;

    private GameObject markerInstance;
    private CursorType currentCursor = CursorType.None;

    public enum CursorType
    {
        None,
        Walkable,
        Selectable,
        Attackable,
        Unavailable
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Keep the real cursor visible so UI detection works
        Cursor.visible = true;
    }

    private void Update()
    {
        if (markerInstance == null)
            return;

        // Update marker position using raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            markerInstance.transform.position = worldPos;
        }
    }

    public void SetMarkerType(CursorType type)
    {
        if (type == currentCursor)
            return;

        currentCursor = type;

        // Destroy old marker
        if (markerInstance != null)
            Destroy(markerInstance);

        // Create new marker
        switch (type)
        {
            case CursorType.Walkable:
                markerInstance = Instantiate(walkableCursor);
                break;

            case CursorType.Selectable:
                markerInstance = Instantiate(selectableCursor);
                break;

            case CursorType.Attackable:
                markerInstance = Instantiate(attackableCursor);
                break;

            case CursorType.Unavailable:
                markerInstance = Instantiate(unavailableCursor);
                break;

            case CursorType.None:
                markerInstance = null;
                return;
        }
    }
}
