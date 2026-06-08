using HFHandyUtils;
using HFHandyUtils.Data.Heatmaps;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
///     Handy math functions
///     <br></br>
///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
///     <br><a href="https://halfhand870.notion.site/Mathh-34ad086035d38013af25e09c177023bb">Documentation</a></br>
/// </summary>
public static class Mathh
{
    #region Vector Calculations
    /// <summary>
    ///     Calculates a spread around a unit circle with uniform angles between each point. Length of output is determined by 'amount'
    /// </summary>
    /// <param name="amount">Positions around the unit circle</param>
    /// <returns>List of positions</returns>
    public static List<Vector3> SpreadPositions(int amount)
    {
        // Calculate degree seperation between each position
        float dSeperation = 360 / (float)amount;
        // -> Convert to radians
        float rSeperation = Mathf.Deg2Rad * dSeperation;

        // Hold variables for total positions and current rotation
        List<Vector3> positions = new List<Vector3>();
        float currentRotation = 0;

        // Roll through each entry and add a position
        for(int x = 0; x < amount; x++)
        {
            // Add a new position
            positions.Add(new Vector3(Mathf.Sin(currentRotation), 0, Mathf.Cos(currentRotation)));

            // Increase current rotation
            currentRotation += rSeperation;
        }

        // Return the list
        return positions;
    }

    /// <summary>
    ///     Gets the closest object from 'input' to 'postition'
    /// </summary>
    /// <typeparam name="T">Object Type that extends Monobehaviour</typeparam>
    /// <param name="input">Input objects</param>
    /// <param name="position">Current position</param>
    /// <param name="ignoreSelf">Flag that checks object at input position when true - Default false</param>
    /// <returns>Closest Object</returns>
    public static T GetClosestObject<T>(T[] input, Vector3 position, bool ignoreSelf = false) where T : MonoBehaviour
    {
        // Error check
        if (input.Length <= 0)
        {
            HFLogger.LogError("Tried to get closest without inputting any objects. Input array size is 0");
            return null;
        }

        // Hold a reference to initial length and object
        float minDistance = float.MaxValue;
        T cObject = input[0];
        // Run a loop to check the rest of the objects
        for(int i = 0; i < input.Length; i++)
        {
            // Get the current minimum distance
            float cMinDistance = Vector3.Distance(position, input[i].transform.position);
            // Check for overlap
            if (!ignoreSelf && cMinDistance == 0)
                continue;

            // Compare loop minimum with method minimum
            if (cMinDistance < minDistance)
            {
                // Log information
                minDistance = cMinDistance;
                cObject = input[i];
            }
        }
        // Check if we didnt set our min distance
        if(minDistance == float.MaxValue)
            HFLogger.LogError("GetClosestObject was never set, returning first array value. This error is often caused by inputing a list of nothing but an object at input position. If this is the intended result please set ignoreSelf to true");

        // Return the current object
        return cObject;
    }
    /// <summary>
    ///     Gets the closest object from 'input' to 'postition'
    /// </summary>
    /// <typeparam name="T">Object Type that extends Monobehaviour</typeparam>
    /// <param name="input">Input objects</param>
    /// <param name="position">Current position</param>
    /// <returns>Closest Object</returns>
    public static T GetClosestObject<T>(List<T> input, Vector3 position, bool ignoreSelf = false) where T : MonoBehaviour
    {
        return GetClosestObject(input.ToArray(), position, ignoreSelf);
    }

    /// <summary>
    ///     Gets the speration percent between two vectors. 
    ///     <br></br> 0 - Both vectors are opposite
    ///     <br></br> 1 - Both vectors are identical
    /// </summary>
    /// <returns>Value between 0-1, 1 being two identical vectors and 0 being opposites</returns>
    public static float GetVectorAccuracy(Vector3 primary, Vector3 other)
    {
        // Get the distance between the two vectors
        float cDistance = Vector3.Distance(primary.normalized, other.normalized);
        // Get the accuracy
        return Mathf.Clamp01(1 - (cDistance / 2));
    }

    /// <summary>
    ///     Removes the vertical axis from a vector
    /// </summary>
    /// <param name="input">Input</param>
    /// <returns>Vector with only x and z</returns>
    public static Vector3 RemoveVerticalAxis(Vector3 input)
    {
        return new Vector3(input.x, 0, input.z);
    }

    /// <summary>
    ///     Multiplies two vectors components
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>New vector with components multiplied</returns>
    public static Vector3 MultiplyVectorComponents(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    /// <summary>
    ///     Clamps a vectors components by a minumum and a maximum
    /// </summary>
    /// <param name="input">Clamped vector</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <returns>Clamped Vector</returns>
    public static Vector3 ClampVectorComponents(Vector3 input, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(input.x, min.x, max.x), Mathf.Clamp(input.y, min.y, max.y), Mathf.Clamp(input.z, min.z, max.z));
    }

    /// <summary>
    ///     Makes each axis of a vector positive
    /// </summary>
    /// <param name="input">Input vector</param>
    /// <returns>Vector with positve axis'</returns>
    public static Vector3 Vector_Abs(Vector3 input)
    {
        return new Vector3(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
    }

    /// <summary>
    ///     Gets the average position of points contained within a defined bound
    /// </summary>
    /// <param name="points">Collection</param>
    /// <param name="region">Bounds</param>
    /// <returns>Collection Average in bounds</returns>
    public static Vector3 GetInBoundsCollectionAverage(Vector3[] points, Bounds region)
    {
        Vector3 totalPosition = Vector3.zero;
        int recordedValues = 0;
        foreach(Vector3 point in points)
        {
            if (region.Contains(point))
            {
                totalPosition += point;
                recordedValues++;
            }
        }
        totalPosition /= recordedValues;
        return totalPosition;
    }

    /// <summary>
    ///     Gets the longest axis from an input vector
    /// </summary>
    /// <param name="input">Vector3</param>
    /// <returns>Longest length axis</returns>
    public static float GetLongestAxis(Vector3 input)
    {
        // Return x
        if (input.x >= input.y && input.x >= input.z)
            return input.x;
        // Return y
        if (input.y >= input.x && input.y >= input.z)
            return input.y;
        // Return z
        return input.z;
    }
    /// <summary>
    ///     Gets the shortest axis from an input vector
    /// </summary>
    /// <param name="input">Vector3</param>
    /// <returns>Shortest length axis</returns>
    public static float GetShortestAxis(Vector3 input)
    {
        // Return x
        if (input.x <= input.y && input.x <= input.z)
            return input.x;
        // Return y
        if (input.y <= input.x && input.y <= input.z)
            return input.y;
        // Return z
        return input.z;
    }

    /// <summary>
    ///     Pulls all verticies from the mesh and orients them in global space
    /// </summary>
    /// <param name="target">Target Transform</param>
    /// <param name="mesh">Target Mesh</param>
    /// <returns>List of global position verticies from a mesh</returns>
    public static Vector3[] GetVerticiesFromMesh(Transform transform, Mesh mesh)
    {
        // Error check
        if (transform == null || mesh == null)
            return new Vector3[0];

        // Return a list of modified verticies
        Vector3[] verticies = new Vector3[mesh.vertexCount];
        for (int i = 0; i < verticies.Length; i++)
        {
            verticies[i] = ApplyTransform_Vector3(transform, mesh.vertices[i]);
        }
        return verticies;
    }
    /// <summary>
    ///     Applys a transform to a vector3
    /// </summary>
    /// <param name="transform">Parent transform</param>
    /// <param name="input">World position</param>
    /// <returns></returns>
    public static Vector3 ApplyTransform_Vector3(Transform transform, Vector3 input)
    {
        return transform.rotation * Vector3.Scale(input, transform.lossyScale) + transform.position;
    }

    /// <summary>
    ///     Evaluates a vector as a dimension and outputs a volume
    /// </summary>
    /// <param name="input">Dimension size</param>
    /// <returns>Volume</returns>
    public static float GetVectorVolume(Vector3 input)
    {
        return input.x * input.y * input.z;
    }

    private static readonly Vector3[] s_rightAngleVectors = new Vector3[]
    {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };
    /// <summary>
    ///     Snaps a direction vector to a right angle vector
    /// </summary>
    /// <param name="direction">Input direction</param>
    /// <returns>Closest direction identity</returns>
    public static Vector3 SnapToRightAngle(Vector3 direction)
    {
        // Get a holder for accuracy comparison
        float maxAccuracy = 0;
        Vector3 rDirection = Vector3.zero;

        // Compare vectors
        for (int i = 0; i < s_rightAngleVectors.Length; i++)
        {
            // Get accuracy
            float cAccuracy = GetVectorAccuracy(direction, s_rightAngleVectors[i]);
            // Check if accuracy is higher
            if(maxAccuracy < cAccuracy)
            {
                maxAccuracy = cAccuracy;
                rDirection = s_rightAngleVectors[i];
            }
        }
        return rDirection;
    }
    #endregion
    #region Bound Calculations

    /// <summary>
    ///     Gets the a bounds significant values
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns>Vector3 array - Length 8</returns>
    public static Vector3[] GetBoundSignificantValues(Bounds bounds)
    {
        return new Vector3[]
        {
            // Top Corners
            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            // Bottom Corners
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),

            // Face Centers
            bounds.center + Vector3.right * bounds.extents.x,
            bounds.center + Vector3.left * bounds.extents.x,

            bounds.center + Vector3.up * bounds.extents.y,
            bounds.center + Vector3.down * bounds.extents.y,

            bounds.center + Vector3.forward * bounds.extents.z,
            bounds.center + Vector3.back * bounds.extents.z,

            // Top Centers
            new Vector3(bounds.center.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.center.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.center.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.center.z),
            // Mid Centers
            new Vector3(bounds.max.x, bounds.center.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.center.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.center.y, bounds.min.z),
            // Bottom Centers
            new Vector3(bounds.center.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.center.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.center.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.center.z),

            // Center
            bounds.center
        };
    }

    /// <summary>
    ///     Gets the bounds corners
    /// </summary>
    /// <param name="bounds">Input bounds</param>
    /// <returns>Vector3 array - Length 8</returns>
    public static Vector3[] GetBoundCorners(Bounds bounds)
    {
        return new Vector3[]
        {
            // Top Corners
            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            // Bottom Corners
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.min.z)
        };
    }
    #endregion
    #region Binary Calculations
    /// <summary>
    ///     Converts input binary into a readable string
    /// </summary>
    /// <param name="input">Integer</param>
    /// <returns>Binary string</returns>
    public static string IntToBinaryString(uint input)
    {
        string binaryString = System.Convert.ToString(input, 2);
        int inserts = 0;

        // Space every 4th character
        for (int i = 0; i < binaryString.Length; i++)
        {
            // Check if we are pointing to the forth character
            if((i - inserts) % 4 == 0 && i != 0)
            {
                binaryString = binaryString.Insert(i, " ");
                inserts++;
                i++;
            }
        }

        return binaryString;
    }
    /// <summary>
    ///     Gets the physical length of a binary in Base2
    ///     <br></br> 10 - Returns 2
    ///     <br></br> 101 -  Returns 3
    ///     <br></br> 100 - Returns 3 
    /// </summary>
    /// <param name="input">Input Value</param>
    /// <returns>Physical Length</returns>
    public static int GetBinaryLength(uint input)
    {
        // Track count
        int count = 0;
        uint modifided = input;
        // Keep right shifting until 0
        while (modifided != 0)
        {
            modifided >>= 1;
            count++;
        }
        // Return count
        return count;
    }

    /// <summary>
    ///     Gets a range of bits from input
    /// </summary>
    /// <param name="input">Input</param>
    /// <param name="size">Binary Size</param>
    /// <param name="startIndex">Start Index</param>
    /// <param name="length">Range Length</param>
    /// <returns>Binary</returns>
    public static uint GetBinaryRange(uint input, int startIndex, int length)
    {
        // Shift the input right by the start index
        var shiftedBinary = input >> startIndex;
        // Get a mask defined by length
        var mask = (1 << length) - 1;
        // Get the final variable
        var outputBinary = shiftedBinary & mask;
        return (uint)outputBinary;
    }
    /// <summary>
    ///     Uses input to override bits in parent
    /// </summary>
    /// <param name="startIndex">Start index of the ser</param>
    /// <param name="input">Override integer</param>
    /// <param name="parent">Variable to set</param>
    /// <returns></returns>
    public static void SetBinaryRange(int startIndex, uint input, ref uint parent)
    {
        var setLength = GetBinaryLength(input);
        // Get the right and left side of the parent
        var parentRight = (uint)((1 << startIndex) - 1) & parent;

        // Get a modified final
        var parentModified = parent >> startIndex + setLength; // Setup the left side
        parentModified <<= setLength; // Reveal override bits
        parentModified += input; // Apply new bits
        parentModified <<= startIndex; // Reveal start index
        parentModified += parentRight; // Restore right side

        // Set the parent
        parent = parentModified;
    }
    /// <summary>
    ///     Resets a range of bits in parent
    /// </summary>
    /// <param name="startIndex">Start index</param>
    /// <param name="length">Length</param>
    /// <param name="parent">Variable to set</param>
    public static void ClearBinaryRange(int startIndex, int length, ref uint parent)
    {
        // Get the right and left side of the parent
        var parentRight = (uint)((1 << startIndex) - 1) & parent;

        // Get a modified final
        var parentModified = parent >> startIndex + length; // Setup the left side
        parentModified <<= length; // Reveal override bits
        parentModified <<= startIndex; // Reveal start index
        parentModified += parentRight; // Restore right side

        // Set the parent
        parent = parentModified;
    }
    #endregion
}
