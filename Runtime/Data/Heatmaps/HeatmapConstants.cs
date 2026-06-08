using UnityEngine;

namespace HFHandyUtils.Data.Heatmaps
{
    public static class HeatmapConstants
    {
        public readonly static Gradient s_ValueGradient = new Gradient()
        {
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0) },
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.blue, 0),
                new GradientColorKey(Color.green, 0.33f),
                new GradientColorKey(Color.yellow, 0.66f),
                new GradientColorKey(Color.red, 1)
            }
        };


        #region Draw Gizmos - Value
        /// <summary>
        ///     Draws a debug Map value. Must be called within OnDrawGizmos
        /// </summary>
        /// <param name="value">Heatmap Value 3D</param>
        /// <param name="mapBounds">Heatmap bounding box</param>
        /// <param name="radiusScale">Radius scale (Default 0)</param>
        public static void Debug_DrawGizmoValue(MapValue value, Bounds mapBounds, float radiusScale = 1)
        {
            Debug_DrawGizmoValue(value.point, value.value, mapBounds, radiusScale);
        }
        /// <summary>
        ///     Draws a debug Map value. Must be called within OnDrawGizmos
        /// </summary>
        /// <param name="point">World position</param>
        /// <param name="value">Point value</param>
        /// <param name="mapBounds">Heatmap bounding box</param>
        /// <param name="radiusScale">Radius scale (Default 0)</param>
        public static void Debug_DrawGizmoValue(Vector3 point, float value, Bounds mapBounds, float radiusScale = 1)
        {
            // Set up radius
            float radius = 0.05f;
            radius *= radiusScale;
            radius *= 1 + value * 0.2f;

            // Draw sphere
            Gizmos.color = s_ValueGradient.Evaluate(value);
            Gizmos.DrawSphere(point, radius);
        }
        #endregion
    }
    [System.Serializable]
    public class MapValue
    {
        public Vector3 point;
        [Range(0, 1)] public float value;

        public MapValue(Vector3 point, float value)
        {
            this.point = point;
            SetValue(value);
        }

        public void AddValue(float value)
        {
            SetValue(this.value + value);
        }
        public void SetValue(float value)
        {
            this.value = Mathf.Clamp01(value);
        }
    }
}