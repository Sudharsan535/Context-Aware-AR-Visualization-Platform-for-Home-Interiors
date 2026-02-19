using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FurniturePlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlacementIndicatorController placementIndicator;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private EnvironmentColorSampler colorSampler;

    [Header("Furniture")]
    [SerializeField] private GameObject furniturePrefab;

    private readonly List<GameObject> spawnedFurniture = new();

    private void Update()
    {
        if (AppModeController.Instance == null ||
            !AppModeController.Instance.IsPlacementMode())
            return;



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

        // ✅ AUTO COLOR MATCH ON SPAWN
        if (colorSampler != null &&
            colorSampler.TryGetAverageColor(
                new Vector2(Screen.width / 2f, Screen.height / 2f),
                out Color sampledColor))
        {
            ApplyColorToFurniture(furniture, sampledColor);
        }

        spawnedFurniture.Add(furniture);

        AppModeController.Instance.SetEditMode();
    }

    private void ApplyColorToFurniture(GameObject furniture, Color color)
    {
        Renderer renderer = furniture.GetComponentInChildren<Renderer>();

        if (renderer != null)
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
