using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    public ResourceManager.ResourcesType resourceType;
    public int totalAmount = 200;       // total resource in the node
    public int gatherPerHit = 5;        // how much a worker gathers per action

    public bool IsDepleted => totalAmount <= 0;

    private void Start()
    {
        // ⭐ Only snap NON-mana resources to terrain
        if (resourceType != ResourceManager.ResourcesType.Mana)
        {
            Vector3 pos = transform.position;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
            transform.position = pos;
        }
        // ⭐ Mana wells keep the exact pivot height set by the spawner
    }

    // Called by workers or gathering systems
    public int Gather()
    {
        if (IsDepleted)
            return 0;

        int gathered = Mathf.Min(gatherPerHit, totalAmount);
        totalAmount -= gathered;

        if (IsDepleted)
            OnDepleted();

        return gathered;
    }

    private void OnDepleted()
    {
        Debug.Log($"[ResourceNode] {resourceType} node depleted at {transform.position}");
        Destroy(gameObject);
    }
}
