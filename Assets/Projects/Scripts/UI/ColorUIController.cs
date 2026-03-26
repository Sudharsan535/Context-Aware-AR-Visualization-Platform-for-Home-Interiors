using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorUIController : MonoBehaviour
{
    public static ColorUIController Instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform swatchContainer;
    [SerializeField] private GameObject swatchPrefab;
    [SerializeField] private GameObject partButtonsRoot;

    private FurnitureMaterialController currentTarget;

    private List<GameObject> spawnedSwatches = new();

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // ==============================
    // SHOW PANEL
    // ==============================
    public void Show(FurnitureMaterialController target)
    {
        currentTarget = target;

        // ✅ SHOW only if needed
        if (partButtonsRoot != null)
            partButtonsRoot.SetActive(target.HasMultipleParts());

        GenerateColorSwatches();

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    // ==============================
    // PART SELECTION
    // ==============================
    public void SelectFrame()
    {
        currentTarget?.SelectFrame();
    }

    public void SelectMattress()
    {
        currentTarget?.SelectMattress();
    }

    public void SelectBlanket()
    {
        currentTarget?.SelectBlanket();
    }

    // ==============================
    // COLOR GENERATION
    // ==============================
    private void GenerateColorSwatches()
    {
        ClearSwatches();

        Color[] colors = new Color[]
        {
            Color.white,
            Color.black,
            Color.gray,
            Color.red,
            Color.blue,
            Color.green,
            new Color(0.6f, 0.3f, 0.1f), // brown
            new Color(1f, 0.8f, 0.6f),   // beige
        };

        foreach (Color color in colors)
        {
            GameObject swatch = Instantiate(swatchPrefab, swatchContainer);

            Image img = swatch.GetComponent<Image>();
            img.color = color;

            Button btn = swatch.GetComponent<Button>();
            btn.onClick.AddListener(() => ApplyColor(color));

            spawnedSwatches.Add(swatch);
        }
    }

    private void ApplyColor(Color color)
    {
        currentTarget?.ApplyColor(color);
    }

    private void ClearSwatches()
    {
        foreach (var s in spawnedSwatches)
        {
            if (s != null)
                Destroy(s);
        }

        spawnedSwatches.Clear();
    }
}