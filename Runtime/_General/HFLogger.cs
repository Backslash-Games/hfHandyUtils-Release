using System.Collections.Generic;
using UnityEngine;

namespace HFHandyUtils
{
    public static class HFLogger
    {
        /// <summary>
        ///     Flag that track if stack trace mode has been set
        /// </summary>
        private static bool _stackTraceSet = false;

        /// <summary>
        ///     Sends information to Unity's Console
        /// </summary>
        public static void Log(object message)
        {
            #if UNITY_EDITOR
            if (_stackTraceSet)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                _stackTraceSet = false;
            }
            Debug.Log(message);
            #endif
        }

        /// <summary>
        ///     Sends information to Unity's Console
        /// </summary>
        public static void LogError(object message)
        {
            #if UNITY_EDITOR
            if (_stackTraceSet)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                _stackTraceSet = false;
            }
            Debug.LogError(message);
            #endif
        }

        #region Time Logging
        /// <summary>
        ///     Structure that tracks the average of a collection of longs
        /// </summary>
        public class AverageTracker 
        {
            private static int s_maximumEntries = 1000;
            private List<long> _entries = new List<long>();

            /// <summary>
            ///     Record an entry in the average
            /// </summary>
            /// <param name="value">Input value</param>
            public void RecordEntry(long value)
            {
                // Adds an entry
                _entries.Add(value);

                // Checks if we are capped out on entries
                if(_entries.Count > s_maximumEntries)
                    _entries.RemoveAt(0);
            }

            /// <summary>
            ///     Pulls the sum of all entries
            /// </summary>
            /// <returns>Sum of all entries</returns>
            public long GetSum()
            {
                long sum = 0;
                foreach (long entry in _entries)
                    sum += entry;
                return sum;
            }
            /// <summary>
            ///     Pulls the average from 
            /// </summary>
            /// <returns>Average</returns>
            public decimal GetAverage()
            {
                return GetSum() / _entries.Count;
            }

            /// <summary>
            ///     Resets average tracker
            /// </summary>
            public void Reset()
            {
                _entries.Clear();
            }
        }

        /// <summary>
        ///     Sends information to Unity's Console, specific format for stopwatches
        /// </summary>
        /// <param name="timer">Input timer</param>
        public static void LogTime(string source, long time)
        {
            float ms = time / System.Diagnostics.Stopwatch.Frequency;
            float ns = ms * 1000000;

            Log($"{source} Timer: {time} ticks ({ms}ms)({ns}ns)");
        }
        /// <summary>
        ///     Sends information to Unity's Console, specific format for stopwatches
        /// </summary>
        /// <param name="timer">Input timer</param>
        public static void LogTime(string source, long time, AverageTracker averageTracker)
        {
            float ms = time / System.Diagnostics.Stopwatch.Frequency;
            float ns = ms * 1000000;

            averageTracker.RecordEntry(time);

            float avg_time = (float)averageTracker.GetAverage();
            float avg_ms = avg_time / System.Diagnostics.Stopwatch.Frequency;
            float avg_ns = avg_ms * 1000000;

            Log($"{source} Timer: {time} ticks ({ms}ms)({ns}ns)\nAverage: {avg_time} ticks ({avg_ms}ms)({avg_ns}ns)");
        }
        #endregion
    }
}