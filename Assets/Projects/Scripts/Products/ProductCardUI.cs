using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductCardUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text title;

    private FurnitureData data;

    public void Setup(FurnitureData d)
    {
        data = d;

        image.sprite = data.previewImage;
        title.text = data.furnitureName;
    }

    public void OnClick()
    {
        ProductDetailUI.Instance.Show(data);
    }
}