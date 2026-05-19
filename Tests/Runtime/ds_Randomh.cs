using UnityEngine;
using HFHandyUtils.Math;


namespace HFHandyUtils.DebugScripts
{
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