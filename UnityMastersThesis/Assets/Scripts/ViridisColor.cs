using UnityEngine;

public static class ViridisColor
{

    public static Color GetColor(float value)
    {
        return GetGradient().Evaluate(Mathf.Clamp01(value));
    }
    
    private static Gradient GetGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[5];
        colorKeys[0] = new GradientColorKey(new Color(0.267f, 0.004f, 0.329f), 0.00f); // Purple
        colorKeys[1] = new GradientColorKey(new Color(0.224f, 0.231f, 0.522f), 0.25f); // Blue
        colorKeys[2] = new GradientColorKey(new Color(0.128f, 0.431f, 0.557f), 0.50f); // Teal
        colorKeys[3] = new GradientColorKey(new Color(0.133f, 0.675f, 0.459f), 0.75f); // Green
        colorKeys[4] = new GradientColorKey(new Color(0.992f, 0.906f, 0.144f), 1.00f); // Yellow

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}