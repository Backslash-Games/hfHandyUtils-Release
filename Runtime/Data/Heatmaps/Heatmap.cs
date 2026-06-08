using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HFHandyUtils.Data.Heatmaps
{
    [System.Serializable]
    public class Heatmap
    {
        public List<MapValue> mapValues = new List<MapValue>();
        public Bounds bounds = new Bounds();

        private static readonly float pointMergeDistance = 0.01f;

        #if HFHANDY_DEVELOPMENT
        public bool drawArray = false; // Flag to see if we even draw an array
        [Range(0.1f, 1)] public float da_accuracy = 0.1f; // Percentage accuracy of the debug array
        #endif

        #region Sequencing
        /// <summary>
        ///     Initializes the heatmap
        /// </summary>
        private void Initialize()
        {
            BuildBounds();
        }
        #endregion

        #region Bound Handling
        /// <summary>
        ///     Builds the bounds
        /// </summary>
        private void BuildBounds()
        {
            bounds = new Bounds();
            foreach (MapValue value in mapValues)
            {
                EncapsulateValue(value);
            }
        }
        /// <summary>
        ///     Encapsulates a new value
        /// </summary>
        /// <param name="value">Input value</param>
        private void EncapsulateValue(MapValue value)
        {
            // Check if we are the first point
            if (bounds == new Bounds())
            {
                bounds.center = value.point;
                return;
            }

            // Otherwise encapsulate
            bounds.Encapsulate(value.point);
        }
        #endregion
        #region Value Handling
        /// <summary>
        ///     Adds a value to map values
        /// </summary>
        /// <param name="point">New Position</param>
        /// <param name="value">New Value</param>
        public void AddValue(Vector3 point, float value, bool compounding = false)
        {
            AddValue(new MapValue(point, value), compounding);
        }
        /// <summary>
        ///     Adds a value to map values, updates bounds
        /// </summary>
        /// <param name="value">New Map Value</param>
        public void AddValue(MapValue value, bool compounding = false)
        {
            // Check if we need to compound
            int index = GetPointIndex(value.point);
            if (IsValueIndex(index))
            {
                if (compounding)
                    mapValues[index].AddValue(value.value);
                return;
            }

            // Otherwise add a new element
            mapValues.Add(value);
            EncapsulateValue(value);
        }
        /// <summary>
        ///     Adds a value to a collection of points
        /// </summary>
        /// <param name="points">Input points</param>
        /// <param name="value">Input value</param>
        /// <param name="compounding">True if we want it to compound with alike points</param>
        public void AddValues(Vector3[] points, float value = 0, bool compounding = false)
        {
            foreach(Vector3 point in points)
                AddValue(point, value, compounding);
        }

        /// <summary>
        ///     Removes a value from the heatmap
        /// </summary>
        /// <param name="value">Input value</param>
        public void RemoveValue (MapValue value)
        {
            int index = GetPointIndex(value.point);
            if (IsValueIndex(index))
            {
                mapValues.RemoveAt(index);
                // Recalculate bounds
                BuildBounds();
            }
        }

        /// <summary>
        ///     Returns a value (0 - 1) at any given point inside the bounds
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>Heatmap influence (0 - 1)</returns>
        public float Evaluate(Vector3 point)
        {
            float wv_sum = 0; // Weighted value sum
            float w_sum = 0; // Weight sum

            foreach(MapValue value in mapValues)
            {
                float dc = Vector3.Distance(point, value.point); // Current distance
                // Error check
                if (dc <= 0) return 0;
                // Check if we are pushing threshold
                if (dc >= bounds.extents.magnitude) continue;

                float wc = 1 / (dc * dc); // Current weight

                // Add to weight value sum
                wv_sum += value.value * wc;
                // Add to weight sum
                w_sum += wc;
            }
            // Error check
            if(w_sum == 0) return 0;
            // Return the value at the current point
            return wv_sum / w_sum;
        }
        /// <summary>
        ///     Evaluates a point and adds it to the list
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>Heatmap influence (0 - 1)</returns>
        public float AddEvaluation(Vector3 point, bool compounding = false)
        {
            float evaluation = Evaluate(point);
            AddValue(point, evaluation, compounding);
            return evaluation;
        }
        /// <summary>
        ///     Evaluates a point and adds it to the list
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>Heatmap influence (0 - 1)</returns>
        public void AddEvaluations(Vector3[] points, bool compounding = false)
        {
            foreach(Vector3 vector3 in points)
                AddEvaluation(vector3, compounding);
        }

        /// <summary>
        ///     Isolates significant values within the heatmap
        /// </summary>
        public void IsolateCorners()
        {
            IsolateValues(Mathh.GetBoundCorners(bounds));
        }
        /// <summary>
        ///     Isolates significant values within the heatmap
        /// </summary>
        public void IsolateSignificant()
        {
            IsolateValues(Mathh.GetBoundSignificantValues(bounds));
        }
        /// <summary>
        ///     Prunes all map values except for input
        /// </summary>
        /// <param name="points"></param>
        public void IsolateValues(Vector3[] points)
        {
            MapValue[] tValues = new MapValue[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                // Check if the value already exists
                int index = GetPointIndex(points[i]);
                if (IsValueIndex(index))
                    tValues[i] = mapValues[index];
                // Otherwise add an evaluation
                else
                    tValues[i] = new MapValue(points[i], Evaluate(points[i]));
            }

            mapValues = new List<MapValue>(tValues);
        }

        /// <summary>
        ///     Resets the heatmap
        /// </summary>
        public void Reset()
        {
            mapValues = new List<MapValue>();
            bounds = new Bounds();
        }
        #endregion

        #region Collection Handling
        /// <summary>
        ///     Gets the index of a point in map values. Has a distance threshold 'pointMergeDistance' set as static readonly
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>Index which contains point (-1 on failure)</returns>
        public int GetPointIndex(Vector3 point)
        {
            for (int i = 0; i < mapValues.Count; i++)
                if (Vector3.Distance(mapValues[i].point, point) <= pointMergeDistance)
                    return i;
            return -1;
        }
        /// <summary>
        ///     Checks if map values has a value at a given point
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>True when a value is found</returns>
        public bool HasValueAtPoint(Vector3 point)
        {
            return IsValueIndex(GetPointIndex(point));
        }
        /// <summary>
        ///     Checks if the given index is a valid value index
        /// </summary>
        /// <param name="index">Input index</param>
        /// <returns>True if valid</returns>
        public bool IsValueIndex(int index)
        {
            return index >= 0 && index < mapValues.Count;
        }

        public bool HasValues()
        {
            return mapValues.Count > 0;
        }
        #endregion
        #region Hottest Point
        /// <summary>
        ///     Gets the hottest value
        /// </summary>
        /// <returns></returns>
        public MapValue GetHottestValue()
        {
            // Check if map values are set
            if (mapValues.Count <= 0)
            {
                HFLogger.LogError("Please set map values before trying to get hottest");
                return new MapValue(Vector3.zero, 0);
            }

            // Store the hottest index and distance
            int hottestIndex = 0;
            float hottestDistance = Vector3.Distance(mapValues[hottestIndex].point, bounds.center);
            
            // Go through each point and look for the hottest
            for(int i = 1; i < mapValues.Count; i++)
            {
                float cDist = Vector3.Distance(mapValues[hottestIndex].point, bounds.center);
                if (mapValues[i].value >= mapValues[hottestIndex].value || (mapValues[i].value == mapValues[hottestIndex].value && cDist < hottestDistance))
                {
                    hottestIndex = i;
                    hottestDistance = cDist;
                }
            }

            return mapValues[hottestIndex];
        }
        #endregion

        /// <summary>
        ///     Debug method. Use only in OnDrawGizmos
        /// </summary>
        public void Debug_DrawGizmos()
        {
            Initialize();

            // Draw bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            // Draw points
            foreach (MapValue value in mapValues)
            {
                HeatmapConstants.Debug_DrawGizmoValue(value, bounds);
                #if HFHANDY_DEVELOPMENT
                Handles.Label(value.point + Vector3.up * 0.15f, value.value.ToString());
                #endif
            }

            // Debug array
            #if HFHANDY_DEVELOPMENT
            if (drawArray)
            {
                da_accuracy = Mathf.Clamp(da_accuracy, 0.1f, float.MaxValue);
                for (float x = bounds.center.x - bounds.extents.x; x <= bounds.center.x + bounds.extents.x; x += da_accuracy)
                {
                    for (float y = bounds.center.y - bounds.extents.y; y <= bounds.center.y + bounds.extents.y; y += da_accuracy)
                    {
                        for (float z = bounds.center.z - bounds.extents.z; z <= bounds.center.z + bounds.extents.z; z += da_accuracy)
                        {
                            Vector3 point = new Vector3(x, y, z);
                            HeatmapConstants.Debug_DrawGizmoValue(point, Evaluate(point), bounds, 0.25f);
                        }
                    }
                }
            }
            #endif
        }
    }
}