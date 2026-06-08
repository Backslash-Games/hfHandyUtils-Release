using System;
using UnityEngine;

namespace HFHandyUtils.Data.Random
{
    /// <summary>
    ///     Creates a thread safe, highly optimized, and instantiable set of random numbers
    ///     Supported types include: byte, ushort, uint
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/RandomSet-359d086035d3804cb8c8d4529518fc13">Documentation</a></br>
    /// </summary>
    /// <typeparam name="TNumeric">Must be a { byte, ushort, uint }</typeparam>

    [Serializable]
    public class RandomSet<TNumeric>
        where TNumeric : IComparable,
                            IComparable<TNumeric>,
                            IConvertible,
                            IEquatable<TNumeric>,
                            IFormattable
    {
        /// <summary>
        ///     Name of the random set. Impacts _localSeed
        /// </summary>
        private string _name = string.Empty;
        /// <summary>
        ///     Length of the set
        /// </summary>
        private int _length = 0;

        /// <summary>
        ///     Maximum value of the set
        /// </summary>
        private CacheValue<uint> _maxValue = new CacheValue<uint>();
        /// <summary>
        ///     Set values
        /// </summary>
        private CacheValue<TNumeric[]> _values = new CacheValue<TNumeric[]>();

        /// <summary>
        ///     Current position in set
        /// </summary>
        private int _index = 0;
        /// <summary>
        ///     Set wide seed
        /// </summary>
        private int _localSeed = -1;

        /// <summary>
        ///     Flag to track initialization. Used to check for improper usage
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        ///     If allowed by GlobalRandom.s_debugMode then it tracks the amount of realtime it takes for a set to populate
        /// </summary>
        private System.Diagnostics.Stopwatch _populationStopwatch = new System.Diagnostics.Stopwatch();

        #region Constructor
        public RandomSet(string name, int length = 64)
        {
            // Set values
            _name = name;
            _length = length;
            _index = 0;
        }
        #endregion
        #region Sequencing
        /// <summary>
        ///     Initializes the random set. Should be called in awake
        /// </summary>
        public void Initialize()
        {
            // Check for a double initialization
            if (_initialized)
            {
                GlobalRandom.LogError(typeof(TNumeric), _name, $"Attempted a double initialization of {_name}");
                return;
            }

            // Bind event
            GlobalRandom.s_OnGlobalSeedChanged -= SetLocalSeed;
            GlobalRandom.s_OnGlobalSeedChanged += SetLocalSeed;

            // Set flag
            _initialized = true;
        }

        /// <summary>
        ///     Terminates the random set
        /// </summary>
        public void Terminate()
        {
            // Check for a double termination
            if (!_initialized)
            {
                GlobalRandom.LogError(typeof(TNumeric), _name, $"Attempted a double termination of {_name}");
                return;
            }

            // Unbind event
            GlobalRandom.s_OnGlobalSeedChanged -= SetLocalSeed;

            // Set flag
            _initialized = false;
        }
        
        /// <summary>
        ///     Checks if we have initialzed the set
        /// </summary>
        /// <returns></returns>
        public bool CheckInitialization()
        {
            if(!_initialized)
                GlobalRandom.LogError(typeof(TNumeric), _name, $"Please initialize {_name} before usage");
            return _initialized;
        }
        #endregion

        #region Seed Management
        /// <summary>
        ///     Sets the local seed
        /// </summary>
        private void SetLocalSeed()
        {
            // Check if we have initialized
            if (!CheckInitialization())
                return;
            // Check if the global seed has been set
            if (!GlobalRandom.s_globalSeed.isSet)
                return;

            // Check if the global seed has changed
            int newSeed = GlobalRandom.s_globalSeed.value + _name.GetHashCode();
            if (newSeed.Equals(_localSeed))
            {
                GlobalRandom.Log(typeof(TNumeric), _name, $"Recieved Identical Seed: {_localSeed}");
                return;
            }

            // Set local value
            _localSeed = newSeed;
            // Debug
            if (GlobalRandom.s_debugMode)
                GlobalRandom.Log(typeof(TNumeric), _name, $"Set Local seed to {_localSeed}");

            // Check for a max value update
            UpdateMaxValue();
            // Populate cache
            Populate();
        }
        #endregion
        #region Set Management
        /// <summary>
        ///     Fills the set with a collection of random values from 0 to _maxValue
        /// </summary>
        private void Populate()
        {
            // Check if we have initialized
            if (!CheckInitialization())
                return;
            // Check if the global seed is set yet
            // -> Unreachable
            if (!GlobalRandom.s_globalSeed.isSet)
            {
                GlobalRandom.SetGlobalSeed(GlobalRandom.GetRandomCluster_DateTime());
                if (GlobalRandom.s_debugMode)
                    GlobalRandom.Log(typeof(TNumeric), _name, "Force set global seed");
                return;
            }


            // Computation Debug - Start
            if (GlobalRandom.s_debugMode)
            {
                // Mark start time
                _populationStopwatch.Restart();
            }


            // Authority - Unlock values and reset index
            _values.Reset();
            _index = 0;

            // Set unity random seed
            //  "I decided to set the unity engine random seed here to ensure nothing 
            //  could interrupt the process before population"
            UnityEngine.Random.InitState(_localSeed);
            // Populate the array
            TNumeric[] population = new TNumeric[_length];
            for (int i = 0; i < population.Length; i++)
                population[i] = Evaluate();
            _values.SetValue(population);


            // Computation Debug - End
            if (GlobalRandom.s_debugMode)
            {
                // Mark end time
                _populationStopwatch.Stop();

                // Debug out
                if (_populationStopwatch.ElapsedTicks != 0)
                    GlobalRandom.Log(typeof(TNumeric), _name, $"Population Time: {_populationStopwatch.ElapsedTicks} ticks ({_populationStopwatch.Elapsed.Milliseconds}ms)({_populationStopwatch.Elapsed.Milliseconds * 1000000}ns)");
            }
        }
        #endregion
        #region Random Logic
        /// <summary>
        ///     Uses unity engine random to generate a new random number. Then converts the number to a valid type TNumeric
        /// </summary>
        /// <returns>Random number of type defined by TNumeric</returns>
        private TNumeric Evaluate()
        {
            // Grab a generated number
            long generated = (long)UnityEngine.Random.Range(0, _maxValue.value);

            // -> Convert to generic
            // Return generated number
            return (TNumeric)Convert.ChangeType(generated, typeof(TNumeric));
        }
        #endregion
        #region Data Management
        /// <summary>
        ///     Interprets the _maxValue that the generic random set will use
        /// </summary>
        private void UpdateMaxValue()
        {
            if (_maxValue.isSet)
                return;

            Type type = typeof(TNumeric);
            // Check types
            for (int i = 0; i < 3; i++)
            {
                if (type.Equals(GlobalRandom.s_validRandomSetTypes[i]))
                {
                    // Create packet based on pre existing
                    _maxValue.SetValue(GlobalRandom.s_packetSizes[i]);
                    return;
                }
            }
            // Throws error
            throw new InvalidProgramException();
        }
        #endregion

        // --> All of these methods will need to be generic
        #region Get Random
        /// <summary>
        ///     Gets a random value between 0 and _maxValue
        /// </summary>
        /// <returns>Random value</returns>
        public TNumeric Next()
        {
            // Check if we have initialized
            if (!CheckInitialization())
                return Evaluate();

            TNumeric cValue = _values.value[_index];
            _index = (_index + 1) % _length;
            return cValue;
        }
        /// <summary>
        ///     Gets a random percentage
        /// </summary>
        /// <returns>Random percentage</returns>
        public float Next_Percentage()
        {
            // Check if we have initialized
            if (!CheckInitialization())
                return 0;

            TNumeric nextValue = Next();
            // Convert to a long
            uint aValue = (uint)nextValue.ToInt64(GlobalRandom.s_parseFormat);

            // Get percentage value
            return (float)aValue / _maxValue.value;
        }

        /// <summary>
        ///     Gets a random value between two ranges
        /// </summary>
        /// <param name="minimum">Minimum range (Inclusive)</param>
        /// <param name="maximum">Maximum range (Exclusive)</param>
        /// <returns>Random value in range</returns>
        public int Range(int minimum, int maximum)
        {
            // Check if we have initialized
            if (!CheckInitialization())
                return 0;

            // Check if minimum is set incorrect
            if (minimum >= maximum)
            {
                Debug.LogError($"Minimum is larger than or equal to maximum range on RandomSet<{typeof(TNumeric)}>::{_name}");
                return 0;
            }

            // Get a percentage
            float percentage = Next_Percentage();

            // Get the distance between the two values
            int randomValue = Mathf.RoundToInt((maximum - minimum) * percentage) + minimum;
            randomValue = Mathf.Clamp(randomValue, minimum, maximum - 1);

            // Return value
            return randomValue;
        }
        /// <summary>
        ///     Gets a random value between two ranges
        /// </summary>
        /// <param name="minimum">Minimum range (Inclusive)</param>
        /// <param name="maximum">Maximum range (Inclusive)</param>
        /// <returns>Random value in range</returns>
        public float Range(float minimum, float maximum)
        {
            // Check if we have initialized
            if (!CheckInitialization())
                return 0;

            // Check if minimum is set incorrect
            if (minimum >= maximum)
            {
                Debug.LogError($"Minimum is larger than or equal to maximum range on RandomSet<{typeof(TNumeric)}>::{_name}");
                return 0;
            }

            // Get a percentage
            float percentage = Next_Percentage();

            // Get the distance between the two values
            float randomValue = ((maximum - minimum) * percentage) + minimum;
            randomValue = Mathf.Clamp(randomValue, minimum, maximum);

            // Return value
            return randomValue;
        }
        #endregion
        #region Get Data
        /// <summary>
        ///     Pulls the raw data currently stored. Used in Unit Test
        /// </summary>
        /// <returns>TNumeric[]</returns>
        public TNumeric[] GetAllValues() { return _values.value; }
        /// <summary>
        ///     Pulls the local seed currently stored. Used in Unit Test
        /// </summary>
        /// <returns>int<returns>
        public int GetLocalSeed() { return _localSeed; }
        #endregion
        #region Debug Prints
        public override string ToString()
        {
            string output = $"RandomSet<{typeof(TNumeric)}>::{_name}  ..  Max Value: {_maxValue.value}\nGlobal Seed: {GlobalRandom.s_globalSeed.value}  ..  Local Seed: {_localSeed}\n";
            output += GetValueString() + "\n";
            return output;
        }
        /// <summary>
        ///     Gets a string of all contained values
        /// </summary>
        /// <returns>String of all values</returns>
        public string GetValueString()
        {
            string output = $"Set Type: {_values.GetType()}\n[";
            for (int i = 0; i < _length; i++)
            {
                output += $"{_values.value[i]}";
                if (i != _length - 1)
                    output += ", ";
                else
                    output += "]";
            }
            return output;
        }
        #endregion
    }
}