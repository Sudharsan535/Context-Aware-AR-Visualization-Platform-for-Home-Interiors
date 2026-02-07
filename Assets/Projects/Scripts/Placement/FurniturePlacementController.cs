using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FurniturePlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlacementIndicatorController placementIndicator;
    [SerializeField] private ARRaycastManager raycastManager;

    [Header("Furniture")]
    [SerializeField] private GameObject furniturePrefab;

    private GameObject spawnedFurniture;
    private static readonly List<ARRaycastHit> hits = new();

    private void Update()
    {
        if (spawnedFurniture != null)
            return;

        if (!placementIndicator.IsPlacementValid())
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        PlaceFurniture();
    }

    private void PlaceFurniture()
    {
        Pose pose = placementIndicator.GetPlacementPose();

        spawnedFurniture = Instantiate(
            furniturePrefab,
            pose.position,
            pose.rotation
        );
    }

    public GameObject GetPlacedFurniture()
    {
        return spawnedFurniture;
    }

    public void ResetPlacement()
    {
        if (spawnedFurniture != null)
            Destroy(spawnedFurniture);

        spawnedFurniture = null;
    }
}