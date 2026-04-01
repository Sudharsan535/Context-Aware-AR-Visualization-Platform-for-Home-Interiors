using UnityEngine;

public class FurnitureMaterialController : MonoBehaviour
{
    [Header("Multi-Part (Optional - Bed)")]
    [SerializeField] private Renderer frameRenderer;
    [SerializeField] private Renderer mattressRenderer;
    [SerializeField] private Renderer blanketRenderer;

    [Header("Single / Group Objects")]
    [SerializeField] private Renderer defaultRenderer;

    private Renderer currentTarget;

    // 🔥 NEW: Auto-detected group renderers
    private Renderer[] groupRenderers;

    private void Awake()
    {
        InitMaterial(frameRenderer);
        InitMaterial(mattressRenderer);
        InitMaterial(blanketRenderer);
        InitMaterial(defaultRenderer);

        // ✅ AUTO COLLECT ALL CHILD RENDERERS (for chairs, sofas, etc.)
        groupRenderers = GetComponentsInChildren<Renderer>();

        foreach (var r in groupRenderers)
        {
            r.material = new Material(r.material);
        }

        // ✅ DEFAULT TARGET LOGIC
        if (blanketRenderer != null)
            currentTarget = blanketRenderer; // Bed
        else if (defaultRenderer != null)
            currentTarget = defaultRenderer; // Simple objects
    }

    void InitMaterial(Renderer r)
    {
        if (r != null)
            r.material = new Material(r.material);
    }
    void Start()
    {
        ContextData test = new ContextData
        {
            dominantColor = Color.red,
            brightness = 0.5f,
            environmentType = EnvironmentType.Normal
        };

        AIColorUIController.Instance.ShowSuggestions(test, null);
    }

    // ==============================
    // BED PART CONTROL
    // ==============================

    public void SelectFrame()
    {
        if (frameRenderer != null)
            currentTarget = frameRenderer;
    }

    public void SelectMattress()
    {
        if (mattressRenderer != null)
            currentTarget = mattressRenderer;
    }

    public void SelectBlanket()
    {
        if (blanketRenderer != null)
            currentTarget = blanketRenderer;
    }

    // ==============================
    // APPLY COLOR
    // ==============================

    public void ApplyColor(Color color)
    {
        // ✅ CASE 1: BED (specific part)
        if (HasMultipleParts() && currentTarget != null)
        {
            ApplyToRenderer(currentTarget, color);
            return;
        }

        // ✅ CASE 2: GROUP OBJECT (chair, sofa, etc.)
        if (groupRenderers != null && groupRenderers.Length > 0)
        {
            foreach (var r in groupRenderers)
            {
                ApplyToRenderer(r, color);
            }
        }
    }

    void ApplyToRenderer(Renderer r, Color color)
    {
        if (r != null && r.material.HasProperty("_BaseColor"))
        {
            r.material.SetColor("_BaseColor", color);
        }
    }

    // ==============================
    // UI CONTROL
    // ==============================

    public bool HasMultipleParts()
    {
        int count = 0;

        if (frameRenderer != null) count++;
        if (mattressRenderer != null) count++;
        if (blanketRenderer != null) count++;

        return count > 1;
    }
}