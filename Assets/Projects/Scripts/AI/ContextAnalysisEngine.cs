using UnityEngine;

public class ContextAnalysisEngine : MonoBehaviour
{
    public static ContextAnalysisEngine Instance;

    [SerializeField] private EnvironmentColorSampler colorSampler;

    private void Awake()
    {
        Instance = this;
    }

    public bool TryAnalyze(out ContextData context)
    {
        context = new ContextData();

        if (colorSampler == null)
            return false;

        // Sample center of screen
        if (!colorSampler.TryGetAverageColor(
                new Vector2(Screen.width / 2f, Screen.height / 2f),
                out Color sampledColor))
        {
            return false;
        }

        // 🎯 SET COLOR
        context.dominantColor = sampledColor;

        // 🎯 CALCULATE BRIGHTNESS
        float brightness = CalculateBrightness(sampledColor);
        context.brightness = brightness;

        // 🎯 CLASSIFY ENVIRONMENT
        context.environmentType = ClassifyEnvironment(brightness);

        return true;
    }

    // ==============================
    // BRIGHTNESS CALCULATION
    // ==============================

    private float CalculateBrightness(Color color)
    {
        // Perceived luminance formula
        return (0.299f * color.r + 0.587f * color.g + 0.114f * color.b);
    }

    // ==============================
    // ENVIRONMENT TYPE
    // ==============================

    private EnvironmentType ClassifyEnvironment(float brightness)
    {
        if (brightness < 0.3f)
            return EnvironmentType.Dark;

        if (brightness > 0.7f)
            return EnvironmentType.Bright;

        return EnvironmentType.Normal;
    }
}