using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // Actual resource storage
    private Dictionary<ResourcesType, int> resources = new Dictionary<ResourcesType, int>();

    [Header("UI References")]
    public TextMeshProUGUI manaUI;
    public TextMeshProUGUI foodUI;
    public TextMeshProUGUI woodUI;
    public TextMeshProUGUI stoneUI;

    // Event for UI updates
    public event Action OnResourceChanged;

    public enum ResourcesType
    {
        Mana,
        Food,
        Wood,
        Stone
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Initialize all resources to 0
        foreach (ResourcesType type in Enum.GetValues(typeof(ResourcesType)))
            resources[type] = 0;
    }

    private void Start()
    {
        UpdateUI();
    }

    // ⭐ Add resources
    public void IncreaseResource(ResourcesType resource, int amount)
    {
        resources[resource] += amount;
        OnResourceChanged?.Invoke();
    }

    // ⭐ Spend resources
    public void DecreaseResource(ResourcesType resource, int amount)
    {
        resources[resource] -= amount;
        if (resources[resource] < 0)
            resources[resource] = 0;

        OnResourceChanged?.Invoke();
    }

    public void AddResource(ResourcesType resource, int amount)
    {
        IncreaseResource(resource, amount);
    }

    // ⭐ Get resource amount
    public int GetResourceAmount(ResourcesType resource)
    {
        return resources[resource];
    }

    // ⭐ Remove resources based on building requirements
    public void RemoveResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.requirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }

    // ⭐ UI update
    private void UpdateUI()
    {
        if (manaUI != null) manaUI.text = resources[ResourcesType.Mana].ToString();
        if (foodUI != null) foodUI.text = resources[ResourcesType.Food].ToString();
        if (woodUI != null) woodUI.text = resources[ResourcesType.Wood].ToString();
        if (stoneUI != null) stoneUI.text = resources[ResourcesType.Stone].ToString();
    }

    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }
}
