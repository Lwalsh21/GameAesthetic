using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;
    public GameObject groundMarker;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(GameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                SelectByCicking(hit.collider.gameObject);
            }
            else
            {
                DeselectAll();
            }
        }
    }

    private void SelectByCicking(GameObject unit)
    {
        DeselectAll();

        unitsSelected.Add(unit);

        EnableUnitMovement(unit, true);
    }

    private void EnableUnitMovement(GameObject unit, bool movement)
    {
        unit.GetComponent<UnitMovement>()enabled = movement;
    }

    private void DeselectAll()
    {
        throw new NotImplementedException();
    }
}
