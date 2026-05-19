using UnityEngine;
using HFHandyUtils.Data;

namespace HFHandyUtils.Math
{
    /// <summary>
    ///     Handy random control
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    /// </summary>
    public static class Randomh
    {
        #region Weighted Object Struct
        /// <summary>
        ///     Entry to contain a generic type and weight
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        [System.Serializable]
        public struct WeightedEntry<T>
        {
            public T value;
            public int weight;
        }

        /// <summary>
        ///     Library of weighted objects
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        [System.Serializable]
        public struct WeightedLibrary<T>
        {
            public WeightedEntry<T>[] weightedObjects;

            private CacheValue<T[]> cache_Values;
            private CacheValue<int[]> cache_Weights;

            /// <summary>
            ///     Gets a list of values
            /// </summary>
            /// <returns>List of values</returns>
            public T[] GetValues()
            {
                if(cache_Values == null)
                    cache_Values = new CacheValue<T[]>();

                if (cache_Values.isSet)
                    return cache_Values.value;

                // Create a list of weights
                T[] values = new T[weightedObjects.Length];
                for (int i = 0; i < weightedObjects.Length; i++)
                    values[i] = weightedObjects[i].value;
                // Set cache weights
                cache_Values.SetValue(values);

                return values;
            }

            /// <summary>
            ///     Gets a list of weights
            /// </summary>
            /// <returns>List of weights</returns>
            public int[] GetWeights()
            {
                if(cache_Weights == null)
                    cache_Weights = new CacheValue<int[]>();

                if(cache_Weights.isSet)
                    return cache_Weights.value;

                // Create a list of weights
                int[] weights = new int[weightedObjects.Length];
                for(int i = 0; i < weightedObjects.Length; i++)
                    weights[i] = weightedObjects[i].weight;
                // Set cache weights
                cache_Weights.SetValue(weights);

                return weights;
            }

            /// <summary>
            ///     Resets cached data
            /// </summary>
            public void ResetCached()
            {
                cache_Values.Reset();
                cache_Weights.Reset();
            }
        }
        #endregion
        
        #region Weighted Randomness
        /// <summary>
        ///     Method that returns a random weighted object
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="library">Random Object</param>
        /// <returns></returns>
        public static T GetWeightedObject<T>(WeightedLibrary<T> library)
        {
            return GetWeightedObject(library.GetValues(), library.GetWeights());
        }
        /// <summary>
        ///     Method that returns a random weighted object
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="values">Object list</param>
        /// <param name="weights">Weight list</param>
        /// <returns>Random Object</returns>
        public static T GetWeightedObject<T>(T[] values, int[] weights)
        {
            return values[GetWeightedIndex(weights)];
        }



        /// <summary>
        ///     Quick call to grab an index based on a weighted list.
        /// </summary>
        /// <param name="weights">Weighted list</param>
        /// <returns>Randomly chosen index based on weights.</returns>
        public static int GetWeightedIndex(int[] weights)
        {
            // Return with error if weights has a null length
            if (weights.Length <= 0)
            {
                Debug.LogError("Length of weights is invalid (<= 0)");
                return 0;
            }

            // Return early if weights is of length 1
            if (weights.Length == 1)
                return 0;


            // Get the total of the weighted list
            int total = GetWeightedTotal(weights);
            // Get a random value within range
            int rng = Random.Range(0, total);

            // Find the chosen value from the list; ignore the last value
            // -> The last value is used as the default return
            for (int i = 0; i < weights.Length - 1; i++)
            {
                // Check if rng is less than the current index
                // TRUE: Return the current index
                if (rng < weights[i])
                    return i;
                // FALSE: Reduce rng by the value of the current index
                rng -= weights[i];
            }

            return weights.Length - 1;
        }
        
        /// <summary>
        ///     Quick call to grab the total weight based on a weighted list.
        /// </summary>
        /// <param name="weights">Weighted list</param>
        /// <returns>Combined total of weighted list</returns>
        public static int GetWeightedTotal(int[] weights)
        {
            // Return with error if weights has a null length
            if (weights.Length <= 0)
            {
                Debug.LogError("Length of weights is invalid (<= 0)");
                return 0;
            }

            // Return early if weights is of length 1
            if (weights.Length == 1)
                return weights[0];


            // Establish a total
            int total = 0;
            // For each value in weights add to the total
            foreach (int value in weights)
            {
                total += value;
            }
            // Return the total
            return total;
        }
        #endregion
        #region Testing
        public static void Test_WeightedLibraryPercentages<T>(WeightedLibrary<T> library)
        {
            // Pull data
            T[] values = library.GetValues();
            int[] weights = library.GetWeights();
            int totalWeight = GetWeightedTotal(weights);
            // Hold debug string
            string d_output = "";
            // Split each value percentage pair
            for (int i = 0; i < values.Length; i++)
                d_output += $"({values[i].ToString()})[{i}]: {100 * (weights[i] / (float)totalWeight)}%\n";

            // Debug out
            Debug.Log(d_output);
        }
        #endregion
    }
}