using UnityEngine;

public class SellMode : MonoBehaviour
{
    public bool isSelling = false;

    void Update()
    {
        if (!isSelling)
            return;

        if (Input.GetMouseButtonDown(0)) // left-click to sell
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BuildingHealth health = hit.collider.GetComponentInParent<BuildingHealth>();
                if (health != null)
                {
                    health.SellBuilding();
                }
            }
        }
    }
}
