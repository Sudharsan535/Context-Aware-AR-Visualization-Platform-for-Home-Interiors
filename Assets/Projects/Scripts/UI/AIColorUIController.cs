using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AIColorUIController : MonoBehaviour
{
    public static AIColorUIController Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject swatchPrefab;

    private FurnitureMaterialController currentTarget;

    private List<GameObject> spawned = new();

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowSuggestions(ContextData context, FurnitureMaterialController target)
    {
        currentTarget = target;

        Clear();

        var colors = AIColorSuggestion.Instance.GenerateSuggestions(context);

        foreach (var c in colors)
        {
            GameObject swatch = Instantiate(swatchPrefab, container);

            swatch.GetComponent<Image>().color = c;

            swatch.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyColor(c);
            });

            spawned.Add(swatch);
        }

        panel.SetActive(true);
    }

    void ApplyColor(Color color)
    {
        currentTarget?.ApplyColor(color);
    }

    void Clear()
    {
        foreach (var s in spawned)
        {
            if (s != null)
                Destroy(s);
        }

        spawned.Clear();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}