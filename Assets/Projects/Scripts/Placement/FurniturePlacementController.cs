using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class FurniturePlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlacementIndicatorController placementIndicator;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("Furniture")]
    [SerializeField] private GameObject furniturePrefab;

    private readonly List<GameObject> spawnedFurniture = new();

    
    private void Update()
    {
        if (!AppModeController.Instance.IsPlacementMode())
            return;

        // ---------- Debug (on device) ----------
        if (debugText != null)
        {
            debugText.text =
                $"Touches: {Input.touchCount}\n" +
                $"PlacementValid: {placementIndicator.IsPlacementValid()}";
        }

        // ---------- Placement checks ----------
        if (!placementIndicator.IsPlacementValid())
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        // 🔒 IMPORTANT: Do NOT place if touching existing furniture
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

        GameObject furniture = Instantiate(
            furniturePrefab,
            anchor.transform.position,
            anchor.transform.rotation,
            anchor.transform
        );

        spawnedFurniture.Add(furniture);
        AppModeController.Instance.SetEditMode();
    }

    // ---------- Helper ----------
    private bool IsTouchOverFurniture(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        return Physics.Raycast(ray);
    }
}
