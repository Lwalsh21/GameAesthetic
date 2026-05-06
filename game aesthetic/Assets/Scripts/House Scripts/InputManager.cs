using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;

    public event Action OnClicked;
    public event Action OnExit;

    private Vector3 lastPosition;

    private void Update()
    {
        UpdateMouseWorldPosition();

        // UI-safe click
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            OnClicked?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
    {
        // Reliable UI detection
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void UpdateMouseWorldPosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayerMask))
        {
            lastPosition = hit.point;
            return;
        }

        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float distance))
        {
            lastPosition = ray.GetPoint(distance);
        }
    }

    public Vector3 GetSelectedMapPosition()
    {
        return lastPosition;
    }
}
