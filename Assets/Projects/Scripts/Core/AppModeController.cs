using UnityEngine;

public enum AppMode
{
    Placement,
    Edit
}

public class AppModeController : MonoBehaviour
{
    public static AppModeController Instance { get; private set; }

    public AppMode CurrentMode { get; private set; } = AppMode.Placement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPlacementMode()
    {
        CurrentMode = AppMode.Placement;
    }

    public void SetEditMode()
    {
        CurrentMode = AppMode.Edit;
    }

    public bool IsPlacementMode()
    {
        return CurrentMode == AppMode.Placement;
    }

    public bool IsEditMode()
    {
        return CurrentMode == AppMode.Edit;
    }
}