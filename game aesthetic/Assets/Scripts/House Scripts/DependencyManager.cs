using System.Collections.Generic;
using UnityEngine;

public class DependencyManager : MonoBehaviour
{
    public static DependencyManager Instance { get; private set; }

    // Tracks which buildings have been built
    private HashSet<int> builtBuildings = new HashSet<int>();

    // Tracks which events have been triggered
    private HashSet<string> triggeredEvents = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Called when a building is successfully placed
    public void RegisterBuilding(int id)
    {
        if (!builtBuildings.Contains(id))
        {
            builtBuildings.Add(id);
            Debug.Log($"[DependencyManager] Registered building ID {id}");

            RefreshUI();
        }
    }

    // Called when a game event happens (tutorial complete, quest done, etc.)
    public void TriggerEvent(string eventName)
    {
        if (!triggeredEvents.Contains(eventName))
        {
            triggeredEvents.Add(eventName);
            Debug.Log($"[DependencyManager] Event triggered: {eventName}");

            RefreshUI();
        }
    }

    // Checks if all requirements for a building are met
    public bool AreRequirementsMet(ObjectData data)
    {
        // Check building dependencies
        foreach (int requiredID in data.requiredBuildingIDs)
        {
            if (!builtBuildings.Contains(requiredID))
            {
                return false;
            }
        }

        // Check event dependencies
        foreach (string evt in data.requiredEvents)
        {
            if (!triggeredEvents.Contains(evt))
            {
                return false;
            }
        }

        return true;
    }

    public void UnregisterBuilding(int id)
    {
        if (builtBuildings.Contains(id))
        {
            builtBuildings.Remove(id);
            Debug.Log($"[DependencyManager] Unregistered building ID {id}");

            RefreshUI();
        }
    }

    // ⭐ Refreshes all UI build buttons + construction slots
    public void RefreshUI()
    {
        ConstructionSlot[] slots = GameObject.FindObjectsOfType<ConstructionSlot>(true);
        foreach (var slot in slots)
            slot.SendMessage("HandleResourceChange", SendMessageOptions.DontRequireReceiver);

        BuildMenuButton[] menuButtons = GameObject.FindObjectsOfType<BuildMenuButton>(true);
        foreach (var btn in menuButtons)
            btn.RefreshState();
    }

    public bool IsFloor(int id)
    {
        // Add all floor IDs here
        return id == 11;
    }
}
