using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "AR Interior/Furniture Data")]
public class FurnitureData : ScriptableObject
{
    public string furnitureName;

    [TextArea]
    public string description;

    public FurnitureCategory category;

    public Sprite previewImage;

    public GameObject prefab;

    public float width;
    public float depth;
    public float height;
}