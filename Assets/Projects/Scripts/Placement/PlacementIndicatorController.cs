using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicatorController : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject placementIndicator;

    private Pose placementPose;
    private bool placementPoseIsValid;

    private static readonly List<ARRaycastHit> hits = new();

    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void UpdatePlacementPose()
    {
        Vector2 screenCenter = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f
        );

        placementPoseIsValid = raycastManager.Raycast(
            screenCenter,
            hits,
            TrackableType.PlaneWithinPolygon
        );

        if (!placementPoseIsValid)
            return;

        placementPose = hits[0].pose;

        // Align reticle with camera forward (Y-axis only)
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        placementPose.rotation = Quaternion.LookRotation(cameraBearing);
    }

    private void UpdatePlacementIndicator()
    {
        if (!placementPoseIsValid)
        {
            placementIndicator.SetActive(false);
            return;
        }

        placementIndicator.SetActive(true);
        placementIndicator.transform.SetPositionAndRotation(
            placementPose.position,
            placementPose.rotation
        );
    }

    public Pose GetPlacementPose()
    {
        return placementPose;
    }

    public bool IsPlacementValid()
    {
        return placementPoseIsValid;
    }
}
