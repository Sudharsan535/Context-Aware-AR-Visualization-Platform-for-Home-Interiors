using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Collider))]
public class FurnitureInteractionController : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    
    [SerializeField] private EnvironmentColorSampler colorSampler;


    [Header("Visual Feedback")]
    [SerializeField] private Material selectedMaterial;

    [Header("Scale Limits")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;

    private static FurnitureInteractionController activeObject;

    private Renderer objectRenderer;
    private Material defaultMaterial;
    private bool isSelected;

    private float initialPinchDistance;
    private Vector3 initialScale;
    private float initialRotationAngle;

    private static readonly List<ARRaycastHit> hits = new();

    private void Awake()
    {
        objectRenderer = GetComponentInChildren<Renderer>();
        if (objectRenderer != null)
            defaultMaterial = objectRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (!AppModeController.Instance.IsEditMode())
            return;

        if (!isSelected)
            return;

        // Block UI touches
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(0))
            return;

        if (Input.touchCount == 1)
            HandleMove(Input.GetTouch(0));
        else if (Input.touchCount == 2)
            HandleRotateAndScale();
        // Manual color pick (tap release while selected)
        if (isSelected && Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if (colorSampler != null &&
                    colorSampler.TryGetAverageColor(touch.position, out Color sampledColor))
                {
                    ApplyColor(sampledColor);
                }
            }
        }
    }
    private void ApplyColor(Color color)
    {
        if (objectRenderer != null)
            objectRenderer.material.SetColor("_BaseColor", color);
    }


    // ================= Selection =================

    private void OnMouseDown()
    {
        Select();
    }

    private void Select()
    {
        AppModeController.Instance.SetEditMode();
        if (activeObject != null && activeObject != this)
            activeObject.Deselect();

        activeObject = this;
        isSelected = true;

        if (objectRenderer != null && selectedMaterial != null)
            objectRenderer.sharedMaterial = selectedMaterial;
    }

    private void Deselect()
    {
        isSelected = false;

        if (objectRenderer != null && defaultMaterial != null)
            objectRenderer.sharedMaterial = defaultMaterial;

        if (activeObject == this)
            activeObject = null;
    }

    // ================= Move =================

    private void HandleMove(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved)
            return;

        if (raycastManager.Raycast(
                touch.position,
                hits,
                TrackableType.PlaneWithinPolygon))
        {
            transform.position = hits[0].pose.position;
        }
    }

    // ================= Rotate & Scale =================

    private void HandleRotateAndScale()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
        {
            initialPinchDistance = Vector2.Distance(t0.position, t1.position);
            initialScale = transform.localScale;
            initialRotationAngle = GetAngle(t0.position, t1.position);
            return;
        }

        // ----- Scale -----
        float currentDistance = Vector2.Distance(t0.position, t1.position);
        float scaleFactor = currentDistance / initialPinchDistance;

        float targetScale = Mathf.Clamp(
            initialScale.x * scaleFactor,
            minScale,
            maxScale
        );

        transform.localScale = Vector3.one * targetScale;

        // ----- Rotate -----
        float currentAngle = GetAngle(t0.position, t1.position);
        float deltaAngle = currentAngle - initialRotationAngle;

        transform.Rotate(Vector3.up, deltaAngle, Space.World);
        initialRotationAngle = currentAngle;
    }

    private float GetAngle(Vector2 p1, Vector2 p2)
    {
        Vector2 dir = p2 - p1;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
