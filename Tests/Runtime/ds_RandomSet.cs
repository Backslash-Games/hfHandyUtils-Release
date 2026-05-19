using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using HFHandyUtils.Data;
using TMPro;


namespace HFHandyUtils.DebugScripts
{
    public class ds_RandomSet : MonoBehaviour
    {
        public int seed = -1;
        public Vector2 range = Vector2.up * 100;
        [Space]

        private readonly static int setSize = 64;

        public RandomSet<byte> _generation_byte = new RandomSet<byte>("Byte", setSize);
        public RandomSet<byte> _generation_byteD = new RandomSet<byte>("Byte", setSize);
        public RandomSet<byte> _generation_byte2 = new RandomSet<byte>("Byte 1", setSize);
        public RandomSet<byte> _generation_byte3 = new RandomSet<byte>("Byte 2", setSize);

        public RandomSet<ushort> _generation_ushort = new RandomSet<ushort>("Unsigned Short", setSize);

        public RandomSet<uint> _generation_uint = new RandomSet<uint>("Unsigned Integer", setSize);

        public bool supressText = false;
        public TextMeshProUGUI text;

        private List<byte> generatedBytes = new List<byte>();
        private List<ushort> generatedShorts = new List<ushort>();
        private List<uint> generatedInts = new List<uint>();

        private List<float> generatedBytes_Percentage = new List<float>();
        private List<float> generatedShorts_Percentage = new List<float>();
        private List<float> generatedInts_Percentage = new List<float>();

        private List<int> generatedBytes_RangeInt = new List<int>();
        private List<int> generatedShorts_RangeInt = new List<int>();
        private List<int> generatedInts_RangeInt = new List<int>();

        private List<float> generatedBytes_RangeFloat = new List<float>();
        private List<float> generatedShorts_RangeFloat = new List<float>();
        private List<float> generatedInts_RangeFloat = new List<float>();

        private Stopwatch _debugStopwatch = new Stopwatch();

        #region Unity Events
        private void Awake()
        {
            _generation_byte.Initialize();
            _generation_byteD.Initialize();
            _generation_byte2.Initialize();
            _generation_byte3.Initialize();

            _generation_ushort.Initialize();

            _generation_uint.Initialize();
        }
        private void Start()
        {
            Repopulate();
        }
        private void OnDestroy()
        {
            _generation_byte.Terminate();
            _generation_byteD.Terminate();
            _generation_byte2.Terminate();
            _generation_byte3.Terminate();

            _generation_ushort.Terminate();

            _generation_uint.Terminate();
        }
        #endregion

        #region Logic
        public void Repopulate()
        {
            _debugStopwatch.Restart();

            GlobalRandom.SetGlobalSeed(seed);
            ShowGenerationStrings();

            _debugStopwatch.Stop();
            UnityEngine.Debug.Log($"Repopulated ({setSize * 6} values) in {_debugStopwatch.Elapsed.Ticks} ticks ({_debugStopwatch.Elapsed.TotalMilliseconds}ms)({_debugStopwatch.Elapsed.TotalMilliseconds * 1000000}ns)");
        }
        #endregion

        #region Button Functions
        public void Button_Repopulate()
        {
            Repopulate();
        }
        public void Button_GenerateNext()
        {
            _debugStopwatch.Restart();

            generatedBytes.Add(_generation_byte.Next());
            generatedShorts.Add(_generation_ushort.Next());
            generatedInts.Add(_generation_uint.Next());

            ShowGeneratedValues(generatedBytes, generatedShorts, generatedInts);

            _debugStopwatch.Stop();
            UnityEngine.Debug.Log($"GeneratedNext Done in {_debugStopwatch.Elapsed.Ticks} ticks ({_debugStopwatch.Elapsed.TotalMilliseconds}ms)({_debugStopwatch.Elapsed.TotalMilliseconds * 1000000}ns)");
        }
        public void Button_GenerateNextPercentage()
        {
            _debugStopwatch.Restart();

            generatedBytes_Percentage.Add(_generation_byte.Next_Percentage());
            generatedShorts_Percentage.Add(_generation_ushort.Next_Percentage());
            generatedInts_Percentage.Add(_generation_uint.Next_Percentage());

            ShowGenerateAlike(generatedBytes_Percentage, generatedShorts_Percentage, generatedInts_Percentage);

            _debugStopwatch.Stop();
            UnityEngine.Debug.Log($"GenerateNextPercentage Done in {_debugStopwatch.Elapsed.Ticks} ticks ({_debugStopwatch.Elapsed.TotalMilliseconds}ms)({_debugStopwatch.Elapsed.TotalMilliseconds * 1000000}ns)");
        }
        public void Button_GenerateRangeInt()
        {
            _debugStopwatch.Restart();

            generatedBytes_RangeInt.Add(_generation_byte.Range((int)range.x, (int)range.y));
            generatedShorts_RangeInt.Add(_generation_ushort.Range((int)range.x, (int)range.y));
            generatedInts_RangeInt.Add(_generation_uint.Range((int)range.x, (int)range.y));

            ShowGenerateAlike(generatedBytes_RangeInt, generatedShorts_RangeInt, generatedInts_RangeInt);

            _debugStopwatch.Stop();
            UnityEngine.Debug.Log($"GenerateRangeInt Done in {_debugStopwatch.Elapsed.Ticks} ticks ({_debugStopwatch.Elapsed.TotalMilliseconds}ms)({_debugStopwatch.Elapsed.TotalMilliseconds * 1000000}ns)");
        }
        public void Button_GenerateRangeFloat()
        {
            _debugStopwatch.Restart();

            generatedBytes_RangeFloat.Add(_generation_byte.Range(range.x, range.y));
            generatedShorts_RangeFloat.Add(_generation_ushort.Range(range.x, range.y));
            generatedInts_RangeFloat.Add(_generation_uint.Range(range.x, range.y));

            ShowGenerateAlike(generatedBytes_RangeFloat, generatedShorts_RangeFloat, generatedInts_RangeFloat);

            _debugStopwatch.Stop();
            UnityEngine.Debug.Log($"GenerateRangeFloat Done in {_debugStopwatch.Elapsed.Ticks} ticks ({_debugStopwatch.Elapsed.TotalMilliseconds}ms)({_debugStopwatch.Elapsed.TotalMilliseconds * 1000000}ns)");
        }
        #endregion
        #region Text Functions
        private void ShowGenerationStrings()
        {
            if (supressText)
                return;

            string output =
                $"{_generation_byte}\n\n" +
                $"{_generation_byteD}\n\n" +
                $"{_generation_byte2}\n\n" +
                $"{_generation_byte3}\n\n" +

                $"{_generation_ushort}\n\n" +

                $"{_generation_uint}";
            text.text = output;
        }
        private void ShowGeneratedValues(List<byte> b, List<ushort> s, List<uint> i)
        {
            if (supressText)
                return;

            string output = "Bytes\n";
            foreach (byte value in b)
                output += $"{value}, ";

            output += "\n\nUnsigned Shorts\n";
            foreach (ushort value in s)
                output += $"{value}, ";

            output += "\n\nUnsigned Integers\n";
            foreach (uint value in i)
                output += $"{value}, ";

            text.text = output;
        }
        private void ShowGenerateAlike<T>(List<T> b, List<T> s, List<T> i)
        {
            if (supressText)
                return;

            string output = "Bytes - Percent\n";
            foreach (T value in b)
                output += $"{value}, ";

            output += "\n\nUnsigned Shorts - Percent\n";
            foreach (T value in s)
                output += $"{value}, ";

            output += "\n\nUnsigned Integers - Percent\n";
            foreach (T value in i)
                output += $"{value}, ";

            text.text = output;
        }
        #endregion
    }
}