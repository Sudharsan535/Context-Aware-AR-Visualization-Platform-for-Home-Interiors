using System.Collections.Generic;
using UnityEngine;

public class ProductListUI : MonoBehaviour
{
    public static ProductListUI Instance;

    [SerializeField] private Transform container;
    [SerializeField] private GameObject productCardPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowProducts(FurnitureCategory category)
    {
        Clear();

        List<FurnitureData> products =
            FurnitureCatalogManager.Instance.GetFurnitureByCategory(category);

        foreach (var data in products)
        {
            GameObject card = Instantiate(productCardPrefab, container);
            card.GetComponent<ProductCardUI>().Setup(data);
        }

        UIManager.Instance.ShowPanel(UIPanelType.ProductList);
    }

    void Clear()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
    public void BackToHome()
    {
        UIManager.Instance.ShowPanel(UIPanelType.Home);
    }
}