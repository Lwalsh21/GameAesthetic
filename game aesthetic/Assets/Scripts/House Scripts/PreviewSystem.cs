using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        // Clean up any existing preview first
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
    }

    internal void StartShowingRemovePreview()
    {
        ApplyFeedbackToCursor(false);
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Color color = materials[i].color;
                color.a = 0.5f;
                materials[i].color = color;

                materials[i] = previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.green : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.green : Color.red;
        c.a = 1f;
        previewMaterialInstance.EnableKeyword("_EMISSION");

        Color finalColor = c * Mathf.LinearToGammaSpace(1);
        previewMaterialInstance.SetColor("_EmissionColor", finalColor);
    }

    // ⭐ QUICK FIX: Force preview height to Y = 1 (plus your offset)
    private void MovePreview(Vector3 position)
    {
        if (previewObject == null)
            return;

        // Force preview height
        position.y = 1f + previewYOffset;

        previewObject.transform.position = position;
    }
}
