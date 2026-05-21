using System;
using UnityEngine;

namespace HFHandyUtils.Data.Random
{
    /// <summary>
    ///     Definition for global random values. Accessed by RandomSets
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/GlobalRandom-366d086035d38063b99fdbc528eef020">Documentation</a></br>
    /// </summary>
    public static class GlobalRandom
    {
        /// <summary>
        ///     Flag that allows custom random system to print debug information
        /// </summary>
        public readonly static bool s_debugMode = false;
        /// <summary>
        ///     Set of allowed RandomSet types
        /// </summary>
        public readonly static Type[] s_validRandomSetTypes = new Type[] { typeof(byte), typeof(ushort), typeof(uint) };
        /// <summary>
        ///     Maximum size of each numeric type
        /// </summary>
        public readonly static uint[] s_packetSizes = new uint[] { 255, 65535, 4294967295 };

        /// <summary>
        ///     Parsing format used when interpreting data in RandomSet
        /// </summary>
        public readonly static System.Globalization.NumberFormatInfo s_parseFormat = new System.Globalization.CultureInfo("en-UK").NumberFormat;

        /// <summary>
        ///     Current global seed
        /// </summary>
        public static CacheValue<int> s_globalSeed = new CacheValue<int>();

        /// <summary>
        ///     Delegate for dynamic seed operations
        /// </summary>
        public delegate void SeedChanged();
        /// <summary>
        ///     Runs whenever the global seed is changed
        /// </summary>
        public static event SeedChanged s_OnGlobalSeedChanged;

        /// <summary>
        ///     Triggers s_OnGlobalSeedChanged
        /// </summary>
        public static void Invoke_GlobalSeedChanged()
        {
            s_OnGlobalSeedChanged?.Invoke();
        }

        /// <summary>
        ///     Sets s_globalSeed with reset authority. Should be called in Unity Start. The seed must be a positive value
        /// </summary>
        public static void SetGlobalSeed(int seed = -1)
        {
            int newSeed = seed;
            if (seed < 0)
                newSeed = DateTime.Now.Millisecond;

            // Authority - Unlock global
            s_globalSeed.Reset();
            // Set the global
            s_globalSeed.SetValue(newSeed);
            // Debug
            if (s_debugMode)
                Log(null, "STATIC", $"Set Global seed to {newSeed}");


            // Trigger static event
            Invoke_GlobalSeedChanged();
        }

        /// <summary>
        ///     Creates a random value based on date time. Used for random global seed generation
        /// </summary>
        /// <returns>Random number</returns>
        public static int GetRandomCluster_DateTime()
        {
            DateTime time = DateTime.Now;
            return time.Millisecond + time.Minute + time.Second + time.Hour;
        }

        // !!!!! Note: This could be moved to a custom logger in the future !!!!!
        #region Quick Logging
        /// <summary>
        ///     Quickly logs information for RandomSet detailing type, name and text
        /// </summary>
        /// <param name="type">Set Type</param>
        /// <param name="name">Set Name</param>
        /// <param name="text">Output Text</param>
        public static void Log(Type type, string name, string text)
        {
            Debug.Log($"RandomSet<{type}>::{name} - {text}");
        }

        /// <summary>
        ///     Quickly logs error information for RandomSet detailing type, name and text
        /// </summary>
        /// <param name="type">Set Type</param>
        /// <param name="name">Set Name</param>
        /// <param name="text">Output Text</param>
        public static void LogError(Type type, string name, string text)
        {
            Debug.LogError($"RandomSet<{type}>::{name} - {text}");
        }
        #endregion
    }
}
