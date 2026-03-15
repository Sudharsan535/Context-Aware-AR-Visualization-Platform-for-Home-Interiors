using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class FurnitureInteractionController : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private Camera mainCamera;

    private bool isDragging;

    private Vector2 previousTouch1Pos;
    private Vector2 previousTouch2Pos;

    private float minScale = 0.2f;
    private float maxScale = 5f;

    private static readonly List<ARRaycastHit> hits = new();

    private void Awake()
    {
        raycastManager = FindFirstObjectByType<ARRaycastManager>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!AppModeController.Instance.IsEditMode())
            return;

        if (Input.touchCount == 1)
        {
            HandleSingleTouch(Input.GetTouch(0));
        }
        else if (Input.touchCount == 2)
        {
            HandleTwoFingerGesture(
                Input.GetTouch(0),
                Input.GetTouch(1)
            );
        }
    }

    // ==============================
    // ONE FINGER — DRAG TO MOVE
    // ==============================

    private void HandleSingleTouch(Touch touch)
    {
        Ray ray = mainCamera.ScreenPointToRay(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == transform || hit.transform.IsChildOf(transform))
                    {
                        isDragging = true;
                    }
                }

                break;

            case TouchPhase.Moved:

                if (!isDragging)
                    return;

                if (raycastManager.Raycast(
                    touch.position,
                    hits,
                    TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    transform.position = Vector3.Lerp(
                        transform.position,
                        hitPose.position,
                        0.5f
                    );
                }

                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                break;
        }
    }

    // ==============================
    // TWO FINGERS — ROTATE + SCALE
    // ==============================

    private void HandleTwoFingerGesture(Touch touch1, Touch touch2)
    {
        if (touch2.phase == TouchPhase.Began)
        {
            previousTouch1Pos = touch1.position;
            previousTouch2Pos = touch2.position;
            return;
        }

        // ---------- SCALE (Pinch) ----------
        float prevDistance = Vector2.Distance(previousTouch1Pos, previousTouch2Pos);
        float currentDistance = Vector2.Distance(touch1.position, touch2.position);

        if (!Mathf.Approximately(prevDistance, 0))
        {
            float scaleFactor = currentDistance / prevDistance;

            Vector3 newScale = transform.localScale * scaleFactor;
            newScale = Vector3.Max(newScale, Vector3.one * minScale);
            newScale = Vector3.Min(newScale, Vector3.one * maxScale);

            transform.localScale = newScale;
        }

        // ---------- ROTATION (Twist) ----------
        float prevAngle = Mathf.Atan2(
            previousTouch2Pos.y - previousTouch1Pos.y,
            previousTouch2Pos.x - previousTouch1Pos.x
        ) * Mathf.Rad2Deg;

        float currentAngle = Mathf.Atan2(
            touch2.position.y - touch1.position.y,
            touch2.position.x - touch1.position.x
        ) * Mathf.Rad2Deg;

        float angleDelta = currentAngle - prevAngle;

        transform.Rotate(0f, angleDelta, 0f);

        previousTouch1Pos = touch1.position;
        previousTouch2Pos = touch2.position;
    }
}