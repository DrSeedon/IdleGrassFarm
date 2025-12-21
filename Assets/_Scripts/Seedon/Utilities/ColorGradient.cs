using UnityEngine;

public static class ColorGradient
{
    /// <summary>
    /// Создаёт градиент от minColor (при value=0) до maxColor (при value=max)
    /// </summary>
    public static Color GetColor(float value, float min, float max, Color minColor, Color maxColor)
    {
        float normalized = Mathf.Clamp01((value - min) / (max - min));
        return Color.Lerp(minColor, maxColor, normalized);
    }

    /// <summary>
    /// Создаёт трёхцветный градиент: low -> mid -> high
    /// </summary>
    public static Color GetColor(float value, float min, float max, Color lowColor, Color midColor, Color highColor)
    {
        float normalized = Mathf.Clamp01((value - min) / (max - min));

        if (normalized < 0.5f)
        {
            // От low до mid (0-50%)
            return Color.Lerp(lowColor, midColor, normalized * 2f);
        }
        else
        {
            // От mid до high (50-100%)
            return Color.Lerp(midColor, highColor, (normalized - 0.5f) * 2f);
        }
    }

    /// <summary>
    /// Предустановленные градиенты для частых случаев
    /// </summary>
    public static class Presets
    {
        // Для голода: красный -> жёлтый -> зелёный
        public static Color Hunger(float value, float max = 100f)
        {
            return GetColor(value, 0f, max, Color.red, Color.yellow, Color.green);
        }

        // Для жажды: оранжевый -> голубой -> синий
        public static Color Thirst(float value, float max = 100f)
        {
            return GetColor(value, 0f, max, new Color(1f, 0.5f, 0f), Color.cyan, Color.blue);
        }

        // Для здоровья: красный -> жёлтый -> зелёный
        public static Color Health(float value, float max = 100f)
        {
            return GetColor(value, 0f, max, Color.red, Color.yellow, Color.green);
        }

        // Для энергии: серый -> жёлтый -> белый
        public static Color Energy(float value, float max = 100f)
        {
            return GetColor(value, 0f, max, Color.gray, Color.yellow, Color.white);
        }
    }
}

