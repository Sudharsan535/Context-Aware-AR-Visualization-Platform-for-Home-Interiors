using System.Collections.Generic;
using UnityEngine;

public enum UIPanelType
{
    Home,
    ProductList,
    ProductDetail,
    ARView
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public class PanelEntry
    {
        public UIPanelType type;
        public GameObject panel;
    }

    [SerializeField] private List<PanelEntry> panels;

    private Dictionary<UIPanelType, GameObject> panelDict;

    private void Awake()
    {
        Instance = this;

        panelDict = new Dictionary<UIPanelType, GameObject>();

        foreach (var p in panels)
            panelDict.Add(p.type, p.panel);
    }

    public void ShowPanel(UIPanelType type)
    {
        foreach (var panel in panelDict.Values)
            panel.SetActive(false);

        panelDict[type].SetActive(true);
    }
}