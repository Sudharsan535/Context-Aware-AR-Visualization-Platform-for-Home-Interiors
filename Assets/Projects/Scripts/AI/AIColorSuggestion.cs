using UnityEngine;
using System.Collections.Generic;

public class AIColorSuggestion : MonoBehaviour
{
    public static AIColorSuggestion Instance;

    private void Awake()
    {
        Instance = this;
    }

    // 🎯 MAIN FUNCTION
    public List<Color> GenerateSuggestions(Color baseColor)
    {
        List<Color> suggestions = new List<Color>();

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        // 1. Complementary
        suggestions.Add(Color.HSVToRGB((h + 0.5f) % 1f, s, v));

        // 2. Analogous
        suggestions.Add(Color.HSVToRGB((h + 0.08f) % 1f, s, v));
        suggestions.Add(Color.HSVToRGB((h - 0.08f + 1f) % 1f, s, v));

        // 3. Neutral tones
        suggestions.Add(Color.HSVToRGB(h, 0.2f, v)); // soft
        suggestions.Add(Color.HSVToRGB(h, s * 0.3f, v * 0.9f)); // muted

        return suggestions;
    }
}