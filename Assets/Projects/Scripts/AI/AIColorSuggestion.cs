using UnityEngine;
using System.Collections.Generic;

public class AIColorSuggestion : MonoBehaviour
{
    public static AIColorSuggestion Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Color> GenerateSuggestions(ContextData context)
    {
        List<Color> suggestions = new List<Color>();

        Color baseColor = context.dominantColor;

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        // 🎯 ADAPT BASED ON ENVIRONMENT

        switch (context.environmentType)
        {
            case EnvironmentType.Dark:
                // Brighter colors for dark rooms
                suggestions.Add(Color.HSVToRGB(h, s * 0.6f, Mathf.Clamp01(v + 0.3f)));
                suggestions.Add(Color.white);
                break;

            case EnvironmentType.Bright:
                // Softer tones for bright rooms
                suggestions.Add(Color.HSVToRGB(h, s * 0.3f, v * 0.9f));
                suggestions.Add(Color.gray);
                break;

            case EnvironmentType.Normal:
                suggestions.Add(Color.HSVToRGB((h + 0.5f) % 1f, s, v)); // complementary
                suggestions.Add(Color.HSVToRGB((h + 0.1f) % 1f, s, v)); // analogous
                break;
        }

        return suggestions;
    }
}