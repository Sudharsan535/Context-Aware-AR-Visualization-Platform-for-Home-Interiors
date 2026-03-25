using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductDetailUI : MonoBehaviour
{
    public static ProductDetailUI Instance;

    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private FurnitureData current;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(FurnitureData data)
    {
        current = data;

        image.sprite = data.previewImage;
        nameText.text = data.furnitureName;
        descriptionText.text = data.description;

        UIManager.Instance.ShowPanel(UIPanelType.ProductDetail);
    }

    public void OnViewInAR()
    {
        FurnitureCatalogManager.Instance.SelectFurniture(current);

        UIManager.Instance.ShowPanel(UIPanelType.ARView);

        AppModeController.Instance.SetPlacementMode();
    }
    public void BackToList()
    {
        UIManager.Instance.ShowPanel(UIPanelType.ProductList);
    }
    public void ExitAR()
    {
        // Clear placed objects
        FindFirstObjectByType<FurniturePlacementController>()
            .ClearAllFurniture();

        // Reset mode
        AppModeController.Instance.SetPlacementMode();

        // Go back to UI
        UIManager.Instance.ShowPanel(UIPanelType.ProductDetail);
    }
}