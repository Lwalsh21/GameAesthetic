using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public int BuildingID { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    public Vector2Int Size { get; private set; }

    private GridData gridData; // ⭐ store the correct grid reference

    public void Initialize(int id, Vector3Int gridPos, Vector2Int size, GridData gridData)
    {
        BuildingID = id;
        GridPosition = gridPos;
        Size = size;
        this.gridData = gridData;

        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            DestroyBuilding(false);
    }

    public void SellBuilding()
    {
        DestroyBuilding(true);
    }

    private void DestroyBuilding(bool isSell)
    {
        ObjectData data = DatabaseManager.Instance.databseSO.GetObjectByID(BuildingID);

        if (isSell)
        {
            ResourceManager.Instance.AddResource(ResourceManager.ResourcesType.Mana, data.sellValue);
            Debug.Log($"[BuildingHealth] Sold building {data.Name} for {data.sellValue}");
        }

        // ⭐ Remove from dependency system
        DependencyManager.Instance.UnregisterBuilding(BuildingID);

        // ⭐ Free grid space
        gridData.RemoveObjectAt(GridPosition);

        // ⭐ Refresh UI
        DependencyManager.Instance.RefreshUI();

        Destroy(gameObject);
    }
}
