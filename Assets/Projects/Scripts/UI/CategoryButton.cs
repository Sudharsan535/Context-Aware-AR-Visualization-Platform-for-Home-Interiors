using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] private FurnitureCategory category;

    public void OnClick()
    {
        ProductListUI.Instance.ShowProducts(category);
    }
}