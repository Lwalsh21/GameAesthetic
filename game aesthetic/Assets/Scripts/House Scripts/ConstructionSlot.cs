using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionSlot : MonoBehaviour
{
    public Sprite availableSprite;
    public Sprite unAvailableSprite;

    private bool IsAvailable;

    public BuySystem buySystem;
    public int databaseItemID;

    private Image img;
    private Button btn;

    private void Awake()
    {
        img = GetComponent<Image>();
        btn = GetComponent<Button>();
    }

    public void ClickedOnSlot()
    {
        if (IsAvailable)
        {
            buySystem.placementSystem.StartPlacement(databaseItemID);
        }
        else
        {
            Debug.Log($"[ConstructionSlot] Cannot build item {databaseItemID}. Requirements not met.");
        }
    }

    private void UpdateAvailabilityUI()
    {
        if (img == null || btn == null)
        {
            Debug.LogError("ConstructionSlot missing Image or Button component!");
            return;
        }

        img.sprite = IsAvailable ? availableSprite : unAvailableSprite;
        btn.interactable = IsAvailable;
    }

    private void HandleResourceChange()
    {
        // Ensure managers exist
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.databseSO == null)
            return;
        if (ResourceManager.Instance == null)
            return;

        // Validate index
        if (databaseItemID < 0 || databaseItemID >= DatabaseManager.Instance.databseSO.objectsData.Count)
        {
            Debug.LogError("Invalid databaseItemID on " + gameObject.name);
            return;
        }

        ObjectData objectData = DatabaseManager.Instance.databseSO.objectsData[databaseItemID];

        // ⭐ 1. Check resource requirements
        bool resourceRequirementsMet = true;

        foreach (BuildRequirement req in objectData.requirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                resourceRequirementsMet = false;
                break;
            }
        }

        // ⭐ 2. Check dependency requirements
        bool dependencyRequirementsMet = DependencyManager.Instance.AreRequirementsMet(objectData);

        // ⭐ 3. Combine both
        IsAvailable = resourceRequirementsMet && dependencyRequirementsMet;

        UpdateAvailabilityUI();
    }

    private void OnEnable()
    {
        if (ResourceManager.Instance != null && DatabaseManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += HandleResourceChange;
            HandleResourceChange();
        }
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= HandleResourceChange;
    }
}
