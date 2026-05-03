using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }


    private int mana;
    private int gold;
    private int wood;
    private int stone;

    public TextMeshProUGUI manaUI;

    // added .system to this line of code to avoid conflict with UnityEngine.Events.UnityEvent
    public event System.Action OnResourceChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public enum ResourcesType
    {
        Mana,
        Gold,
        Wood,
        Stone
    }

    private void Start()
    {
        UpdateUI();
    }

    public void IncreaseResource(ResourcesType resource, int amountToIncrease)
    {
        switch (resource)
        {
            case ResourcesType.Mana:
                mana += amountToIncrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void DecreaseResource(ResourcesType resource, int amountToDecrease)
    {
        switch (resource)
        {
            case ResourcesType.Mana:
                mana -= amountToDecrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }


    private void UpdateUI()
    {
        manaUI.text = $"{mana}";
    }

    public int Getmana()
    {
        return mana;
    }

    internal int GetResourceAmount(ResourcesType resource)
    {
        switch (resource)
        {
            case ResourcesType.Mana:
                return mana;
            default:
                break;
        }

        return 0;
    }

    internal void RemoveResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.requirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
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
