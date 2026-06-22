using UnityEngine;

namespace HFHandyUtils
{
    public static class Color
    {
        /// <summary>
        ///     Gradient used to show hue
        /// </summary>
        public readonly static Gradient hueGradient = new Gradient()
        {
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0) },
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(UnityEngine.Color.red, 0),
                new GradientColorKey(UnityEngine.Color.orange, 0.143f),
                new GradientColorKey(UnityEngine.Color.yellow, 0.286f),
                new GradientColorKey(UnityEngine.Color.green, 0.429f),
                new GradientColorKey(UnityEngine.Color.blue, 0.572f),
                new GradientColorKey(UnityEngine.Color.indigo, 0.715f),
                new GradientColorKey(UnityEngine.Color.magenta, 0.858f),
                new GradientColorKey(UnityEngine.Color.red, 1)
            }
        };
        /// <summary>
        ///     Gradient used to evaluate heat information
        /// </summary>
        public readonly static Gradient heatGradient = new Gradient()
        {
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0) },
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(UnityEngine.Color.blue, 0),
                new GradientColorKey(UnityEngine.Color.green, 0.33f),
                new GradientColorKey(UnityEngine.Color.yellow, 0.66f),
                new GradientColorKey(UnityEngine.Color.red, 1)
            }
        };
        /// <summary>
        ///     Gradient used to evaluate error information (0 - Error || 1 - Correct)
        /// </summary>
        public readonly static Gradient errorGradient = new Gradient()
        {
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0) },
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(UnityEngine.Color.red, 0),
                new GradientColorKey(UnityEngine.Color.yellow, 0.5f),
                new GradientColorKey(UnityEngine.Color.green, 1)
            }
        };
    }
}