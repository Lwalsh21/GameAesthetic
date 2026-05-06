using UnityEngine;

public class ResourceDropOff : MonoBehaviour
{
    public ResourceManager.ResourcesType[] acceptedResources;
    public Transform dropOffPoint;

    private void Reset()
    {
        dropOffPoint = transform;
    }

    public bool Accepts(ResourceManager.ResourcesType type)
    {
        foreach (var r in acceptedResources)
        {
            if (r == type)
                return true;
        }
        return false;
    }
}
