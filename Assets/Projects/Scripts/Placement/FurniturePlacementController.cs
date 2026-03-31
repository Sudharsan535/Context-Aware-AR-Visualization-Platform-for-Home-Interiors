using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FurniturePlacementController : MonoBehaviour
{
    // ✅ SINGLETON
    public static FurniturePlacementController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlacementIndicatorController placementIndicator;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private TextMeshProUGUI debugText;

    private readonly List<GameObject> spawnedFurniture = new();

    private void Awake()
    {
        // ✅ Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (AppModeController.Instance == null ||
            !AppModeController.Instance.IsPlacementMode())
            return;

        // Debug info (optional)
        if (debugText != null)
        {
            debugText.text =
                $"Touches: {Input.touchCount}\n" +
                $"PlacementValid: {placementIndicator.IsPlacementValid()}";
        }

        if (!placementIndicator.IsPlacementValid())
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (IsTouchOverFurniture(touch))
            return;

        PlaceFurniture();
    }

    private void PlaceFurniture()
    {
        Pose pose = placementIndicator.GetPlacementPose();

        // Create anchor object
        GameObject anchorObject = new GameObject("FurnitureAnchor");
        anchorObject.transform.SetPositionAndRotation(pose.position, pose.rotation);

        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

        if (anchor == null)
        {
            Destroy(anchorObject);
            return;
        }

        FurnitureData selectedData =
            FurnitureCatalogManager.Instance.GetSelectedFurniture();

        if (selectedData == null)
        {
            Debug.LogWarning("No furniture selected.");
            return;
        }

        GameObject furniture = Instantiate(
            selectedData.prefab,
            anchor.transform.position,
            anchor.transform.rotation,
            anchor.transform
        );

        // ✅ CONTEXT-AWARE AI (NEW SYSTEM)
        FurnitureMaterialController materialController =
            furniture.GetComponent<FurnitureMaterialController>();

        if (materialController != null &&
            ContextAnalysisEngine.Instance != null &&
            ContextAnalysisEngine.Instance.TryAnalyze(out ContextData context))
        {
            AIColorUIController.Instance.ShowSuggestions(context, materialController);
        }

        spawnedFurniture.Add(furniture);

        AppModeController.Instance.SetEditMode();
    }

    // ✅ CLEANUP SYSTEM (IMPORTANT FIX)
    public void ClearAllFurniture()
    {
        foreach (var obj in spawnedFurniture)
        {
            if (obj != null)
            {
                // 🔥 Destroy anchor (parent), not just object
                if (obj.transform.parent != null)
                {
                    Destroy(obj.transform.parent.gameObject);
                }
                else
                {
                    Destroy(obj);
                }
            }
        }

        spawnedFurniture.Clear();
    }

    private void ApplyColorToFurniture(GameObject furniture, Color color)
    {
        Renderer renderer = furniture.GetComponentInChildren<Renderer>();

        if (renderer != null && renderer.material.HasProperty("_BaseColor"))
        {
            renderer.material.SetColor("_BaseColor", color);
        }
    }

    private bool IsTouchOverFurniture(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        return Physics.Raycast(ray);
    }
}