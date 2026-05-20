using UnityEngine;
using System.Collections.Generic;
using HFHandyUtils.Time;

namespace HFHandyUtils.Effects
{
    /// <summary>
    ///     Scene definition for playing an effect
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/EffectComponent-34ad086035d380c9b5b2f429d71d9a8a">Documentation</a></br>
    /// </summary>
    [System.Serializable]
    public class EffectComponent : MonoBehaviour
    {
        #region Parameter Class
        /// <summary>
        ///     Defines parameters for a basic effect component
        /// </summary>
        [System.Serializable]
        public class Parameters
        {
            /// <summary>
            ///     Defines the origin of the effect
            /// </summary>
            public Transform origin;
            /// <summary>
            ///     Flag that checks if the effect should follow the origin
            /// </summary>
            public bool sticky = true;
        }
        #endregion
        /// <summary>
        ///      States that correlate with effect sequence
        /// </summary>
        public enum EffectState { None, Initialized, Started, Paused, Stopped, Completed };

        /// <summary>
        ///     Current state of the effect, when changed it triggers the associated event
        /// </summary>
        [Header("Effect Variables")]
        public EffectState effectState = EffectState.None;
        #region Effect State Events
        /// <summary>
        ///     Delegate for effect states changing
        /// </summary>
        public delegate void EffectStateChanged();
        /// <summary>
        ///     Runs when object is initialized
        /// </summary>
        public event EffectStateChanged OnInitialized;
        /// <summary>
        ///     Runs when the effect is started
        /// </summary>
        public event EffectStateChanged OnStarted;
        /// <summary>
        ///     Runs when the effect is paused
        /// </summary>  
        public event EffectStateChanged OnPaused;
        /// <summary>
        ///     Runs when the effect is stopped
        /// </summary>
        public event EffectStateChanged OnStopped;
        /// <summary>
        ///     Runs when the effect is completed
        /// </summary>
        public event EffectStateChanged OnCompleted;
        #endregion
        /// <summary>
        ///     Timer that tracks the allocated alive time of the effect
        /// </summary>
        public Cooldown effectTimer;

        /// <summary>
        ///     Effect identifier. Pulled from inputted component name
        /// </summary>
        private string _identifier = "";
        /// <summary>
        ///     The total amount of duplicate effects allowed at one time.
        /// </summary>
        private static readonly byte s_MaxDuplicates = 15;
        /// <summary>
        ///     Dictionary of duplicate counts organized by Effect Identifier
        /// </summary>
        private static Dictionary<string, byte> s_ActiveDupes;
        /// <summary>
        ///     List of active continuous Effects, only 1 active of each type is allowed at a time
        /// </summary>
        private static List<string> s_ActiveContinuous;

        #region Unity Methods
        /// <summary>
        ///     Default Awake Method. Initializes effectTimer and binds CleanupEffect to OnCooldownSuccess
        /// </summary>
        private void Awake()
        {
            effectTimer = new Cooldown(this, 0, 1);
            effectTimer.OnSuccess += CleanupEffect;
        }
        #endregion

        #region Sequencing
        /// <summary>
        ///     Initializes values when playing the effect
        /// </summary>
        /// <param name="p">Effect Parameters</param>
        public virtual void Initialize(Parameters p)
        {
            // Stick effect to origin
            StickToOrigin(p);

            // Check if we need to make the effect sticky
            if (p.sticky)
                effectTimer.OnUpdate += () => StickToOrigin(p);
        }
        /// <summary>
        ///     Plays effect based on input
        /// </summary>
        /// <param name="target">Target Object</param>
        /// <param name="parameters">Effect Parameters</param>
        /// <param name="eventKey">Event key</param>
        /// <param name="mode">Play mode</param>
        public virtual void Play(object target, Parameters parameters, string eventKey, EffectManager.PlayMode mode)
        {
            Initialize(parameters);
        }
        /// <summary>
        ///     Stops the effect
        /// </summary>
        public virtual void Stop() { CleanupEffect(); }
        #endregion
        #region Object Handling
        /// <summary>
        ///     Cleans up the effect
        /// </summary>
        private void CleanupEffect()
        {
            // Make sure item is no longer present in any list
            RemoveFromDupeDictionary(_identifier);
            RemoveFromContinuous(_identifier);

            // Destroy
            Destroy(gameObject);
        }

        /// <summary>
        ///     Sets the effect position to Parameters.origin
        /// </summary>
        /// <param name="p">Parameters</param>
        private void StickToOrigin(Parameters p)
        {
            transform.position = p.origin.position;
        }
        #endregion

        #region Effect State Handling
        /// <summary>
        ///     Compares other with effectState
        /// </summary>
        /// <param name="other">Other effect state</param>
        /// <returns>True if states are equal</returns>
        public bool CompareEffectState(EffectState other) { return other.Equals(effectState); }

        /// <summary>
        ///     Sets effectState to other, runs events if changed
        /// </summary>
        /// <param name="other">State to move to</param>
        public void SetEffectState(EffectState other)
        {
            // Check if other does not equal our current
            if (CompareEffectState(other))
                return;

            // Set our new state
            effectState = other;

            // Trigger event
            InvokeEffectStateChanged();
        }

        /// <summary>
        ///     Triggers the effect state event based on effectState
        /// </summary>
        private void InvokeEffectStateChanged()
        {
            switch (effectState)
            {
                case EffectState.Initialized:
                    OnInitialized?.Invoke();
                    break;
                case EffectState.Started:
                    OnStarted?.Invoke();
                    break;
                case EffectState.Paused:
                    OnPaused?.Invoke();
                    break;
                case EffectState.Stopped:
                    OnStopped?.Invoke();
                    break;
                case EffectState.Completed:
                    OnCompleted?.Invoke();
                    break;
            }
        }
        #endregion
        #region Set Methods
        /// <summary>
        ///     Sets the value of _identifier to key
        /// </summary>
        /// <param name="key">New Identifier</param>
        public void SetIdentifier(string key) { _identifier = key; }
        #endregion

        #region STATIC - General
        /// <summary>
        ///     Checks if effect is playable
        /// </summary>
        /// <param name="effectName">Name of the effect</param>
        /// <returns>True if the number of effects played hasn't gone over maximum</returns>
        public static bool isPlayable(string effectName)
        {
            // Make sure dictionary is setup
            SetupDupeDictionary();
            SetupContinuousList();

            // --> Continuous checks <--
            if (s_ActiveContinuous.Contains(effectName))
                return false;

            // --> Duplicate checks <--
            // Check if the key exists in the dictionary
            if (s_ActiveDupes.ContainsKey(effectName) && s_ActiveDupes[effectName] >= s_MaxDuplicates)
                return false;

            // Otherwise return false
            return true;
        }
        #endregion
        #region STATIC - Duplicate Handling
        /// <summary>
        ///     Lazy initialization for s_ActiveDupes
        /// </summary>
        private static void SetupDupeDictionary()
        {
            if (s_ActiveDupes == null)
                s_ActiveDupes = new Dictionary<string, byte>();
        }

        /// <summary>
        ///     Adds count of effectName to the dictionary
        /// </summary>
        /// <param name="effectName">Name of the effect</param>
        /// <param name="count">Amount of effect added</param>
        public static void AddToDupeDictionary(string effectName, int count = 1)
        {
            // Make sure we are setup
            SetupDupeDictionary();

            // Check if the key exists in the dictionary
            if (s_ActiveDupes.ContainsKey(effectName))
            {
                s_ActiveDupes[effectName] += (byte)count;
                return;
            }

            // Add new element
            s_ActiveDupes.Add(effectName, (byte)count);
        }
        /// <summary>
        ///     Removes count of effectName the dictionary
        /// </summary>
        /// <param name="effectName">Name of the effect</param>
        /// <param name="count">Amount of effect removed</param>
        public static void RemoveFromDupeDictionary(string effectName, int count = 1)
        {
            // Make sure we are setup
            SetupDupeDictionary();

            // Check if the key exists in the dictionary
            if (!s_ActiveDupes.ContainsKey(effectName))
                return;

            // Remove count from key
            s_ActiveDupes[effectName] = (byte)Mathf.Clamp(s_ActiveDupes[effectName] - count, 0, s_MaxDuplicates);
        }
        #endregion
        #region STATIC - Continuous Handling
        /// <summary>
        ///     Lazy initialization for s_ActiveContinuous
        /// </summary>
        static public void SetupContinuousList()
        {
            if (s_ActiveContinuous == null)
                s_ActiveContinuous = new List<string>();
        }

        /// <summary>
        ///     Adds effectName to the active continuous list
        /// </summary>
        /// <param name="effectName">Effect name</param>
        static public void AddToContinuous(string effectName)
        {
            // Make sure list is setup
            SetupContinuousList();

            // Make sure name isnt in continuous yet
            if (s_ActiveContinuous.Contains(effectName))
                return;

            // Add entry to list
            s_ActiveContinuous.Add(effectName);
        }

        /// <summary>
        ///     Removes entry with effectName from continuous list
        /// </summary>
        /// <param name="effectName">Effect name</param>
        static public void RemoveFromContinuous(string effectName)
        {
            // Make sure list is setup
            SetupContinuousList();

            // Remove entry
            s_ActiveContinuous.Remove(effectName);
        }
        #endregion
    }
}