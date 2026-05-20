using UnityEngine;
using HFHandyUtils.Math;


namespace HFHandyUtils.DebugScripts
{
    /// <summary>
    ///     Debug script for Random H... Should only be used in Unity Inspector
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="">Documentation</a></br>
    /// </summary>
    [AddComponentMenu("HFHandyUtils/Debug/ds_Randomh")]
    public class ds_Randomh : MonoBehaviour
    {
        /// <summary>
        ///     Weighted library declaration
        /// </summary>
        public Randomh.WeightedLibrary<byte> weightedLibrary;

        private void Awake()
        {
            // Percentage testing
            Randomh.Test_WeightedLibraryPercentages(weightedLibrary);

            // Random value testing
            byte randomValue = Randomh.GetWeightedObject(weightedLibrary);
            Debug.Log(randomValue);
        }
    }
}