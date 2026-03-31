using UnityEngine;

public struct ContextData
{
    public Color dominantColor;
    public float brightness; // 0–1
    public EnvironmentType environmentType;
}

public enum EnvironmentType
{
    Dark,
    Normal,
    Bright
}
