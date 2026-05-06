using UnityEngine;

public class WorkerTestAssign : MonoBehaviour
{
    public WorkerGatherer worker;
    public ResourceDropOff dropOff;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // right-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ResourceNode node = hit.collider.GetComponent<ResourceNode>();
                if (node != null)
                {
                    worker.AssignJob(node, dropOff.dropOffPoint);
                }
            }
        }
    }
}
