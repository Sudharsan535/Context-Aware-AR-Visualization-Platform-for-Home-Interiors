using System.Collections.Generic;
using UnityEngine;

public class FurnitureCatalogManager : MonoBehaviour
{
    public static FurnitureCatalogManager Instance { get; private set; }

    [SerializeField] private List<FurnitureData> allFurniture = new();

    private FurnitureData currentSelection;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectFurniture(FurnitureData data)
    {
        currentSelection = data;
    }

    public FurnitureData GetSelectedFurniture()
    {
        return currentSelection;
    }

    public List<FurnitureData> GetFurnitureByCategory(FurnitureCategory category)
    {
        return allFurniture.FindAll(f => f.category == category);
    }
}