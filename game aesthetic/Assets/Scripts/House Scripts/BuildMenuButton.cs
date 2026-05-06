using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockOverlay;

    private ObjectData data;
    private BuySystem buySystem;

    // Initialize with BOTH ObjectData and BuySystem
    public void Initialize(ObjectData data, BuySystem buySystem)
    {
        this.data = data;
        this.buySystem = buySystem;

        label.text = data.Name;

        var sr = data.Prefab.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            icon.sprite = sr.sprite;

        RefreshState();
    }

    public void RefreshState()
    {
        bool resourceRequirementsMet = true;

        // ⭐ Check resource requirements
        foreach (BuildRequirement req in data.requirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                resourceRequirementsMet = false;
                break;
            }
        }

        // ⭐ Check dependency requirements
        bool dependencyRequirementsMet = DependencyManager.Instance.AreRequirementsMet(data);

        // ⭐ Combine both
        bool unlocked = resourceRequirementsMet && dependencyRequirementsMet;

        button.interactable = unlocked;

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);
    }

    public void OnClick()
    {
        // Double-check before starting placement
        bool resourceRequirementsMet = true;

        foreach (BuildRequirement req in data.requirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                resourceRequirementsMet = false;
                break;
            }
        }

        bool dependencyRequirementsMet = DependencyManager.Instance.AreRequirementsMet(data);

        if (!resourceRequirementsMet || !dependencyRequirementsMet)
        {
            Debug.Log($"[BuildMenuButton] Cannot build {data.Name}. Requirements not met.");
            return;
        }

        buySystem.placementSystem.StartPlacement(data.ID);
    }
}
