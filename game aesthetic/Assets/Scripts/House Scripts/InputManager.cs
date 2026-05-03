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

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            OnClicked?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
    {
        // Works even when cursor is hidden or replaced
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void UpdateMouseWorldPosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        // First try: real collider raycast
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayerMask))
        {
            lastPosition = hit.point;
            return;
        }

        // Second try: ground plane fallback
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
