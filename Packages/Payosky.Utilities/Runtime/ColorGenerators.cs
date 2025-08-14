using UnityEngine;

namespace Payosky.Utilities.Colors
{
    /// <summary>
    /// A static utility class providing methods for generating colors.
    /// </summary>
    public static class ColorGenerators
    {
        /// <summary>
        /// Converts a given string into a UnityEngine.Color instance.
        /// </summary>
        /// <param name="input">The string input to be converted into a color. If null or empty, white is returned.</param>
        /// <returns>A UnityEngine.Color instance representing the generated color based on the string's hash code.</returns>
        public static Color StringToColor(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Color.white;
            }

            var seed = input.GetHashCode();
            var random = new System.Random(seed);

            var r = (float)random.NextDouble();
            var g = (float)random.NextDouble();
            var b = (float)random.NextDouble();

            return new Color(r, g, b);
        }

        /// <summary>
        /// Returns a lighter or pastelized version of the color if it's too dark
        /// for good visibility on dark backgrounds.
        /// </summary>
        /// <param name="color">The input color.</param>
        /// <param name="threshold">Brightness threshold (0-1). Default: 0.4</param>
        /// <returns>A lighter color if original is too dark, otherwise the same color.</returns>
        public static Color EnsurePastelForDarkBg(this Color color, float threshold = 0.4f)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);

            if (!(v < threshold)) return color;
            v = Mathf.Lerp(v, 1f, 0.6f);
            s *= 0.6f;
            return Color.HSVToRGB(h, s, v);
        }
    }
}