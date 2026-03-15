using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "AR Interior/Furniture Data")]
public class FurnitureData : ScriptableObject
{
    [Header("Basic Info")]
    public string furnitureName;
    public FurnitureCategory category;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Real World Dimensions (Meters)")]
    public float width;
    public float depth;
    public float height;

    [Header("Allow Color Change")]
    public bool allowColorOverride = true;
}