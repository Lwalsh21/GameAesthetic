using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabseSO : ScriptableObject
{
    public List<ObjectData> objectsData;

    public ObjectData GetObjectByID(int id)
    {
        foreach (ObjectData obj in objectsData)
        {
            if (obj.ID == id)
                return obj;
        }

        Debug.LogError($"[ObjectsDatabseSO] No object with ID {id} found in database!");
        return null;
    }
}

[System.Serializable]
public class ObjectData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField, TextArea(3, 10)] public string description;
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: SerializeField] public List<BuildRequirement> requirements { get; private set; }

    [field: SerializeField] public List<BuildBenefits> benefits { get; private set; }

    [field: SerializeField] public List<int> requiredBuildingIDs { get; private set; } = new List<int>();

    [field: SerializeField] public List<string> requiredEvents { get; private set; } = new List<string>();

    [field: SerializeField] public bool restrictPlacement { get; private set; } = false;

    [field: SerializeField] public int sellValue { get; private set; } = 0;

}

[System.Serializable]
public class BuildRequirement
{
    public ResourceManager.ResourcesType resource;
    public int amount;
}

[System.Serializable]
public class BuildBenefits
{
    public enum BenefitType { Housing }

    public string benefit;
    public Sprite benefitIcon;
    public BenefitType benefitType;
    public int benefitAmount;
}
