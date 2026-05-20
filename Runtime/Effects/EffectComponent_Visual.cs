using UnityEngine;
using UnityEngine.VFX;

namespace HFHandyUtils.Effects
{
    /// <summary>
    ///     Scene definition for playing a visual effect
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/EffectComponent_Visual-34ad086035d380dca2ffda9fcbce0f5a">Documentation</a></br>
    /// </summary>
    [RequireComponent(typeof(VisualEffect))]
    public class EffectComponent_Visual : EffectComponent
    {
        private string _eventKey = "";

        #region Parameter Class
        /// <summary>
        ///     Defines parameters for a visual effect component
        /// </summary>
        [System.Serializable]
        public class VisualParameters : Parameters
        {
            /// <summary>
            ///     Length in time of the particle effect
            /// </summary>
            public float length = 1;
        }
        #endregion

        /// <summary>
        ///     Reference to the visual effect created by the component
        /// </summary>
        [Header("Visual Components")]
        [SerializeField] private VisualEffect source;

        /// <summary>
        ///     Plays effect based on input
        /// </summary>
        /// <param name="target">Target Object</param>
        /// <param name="parameters">Effect Parameters</param>
        /// <param name="eventKey">Event key</param>
        /// <param name="mode">Play mode</param>
        public override void Play(object target, Parameters parameters, string eventKey, EffectManager.PlayMode mode)
        {
            base.Play(target, parameters, eventKey, mode);

            // Try to parse the target
            if (target is not VisualEffectAsset)
            {
                Debug.LogError("Attempted to play non visual on visual effect component");
                return;
            }

            // Setup
            VisualEffectAsset asset = (VisualEffectAsset)target;
            GetVisualSource(); // Ensure visual source is set properly
            source.visualEffectAsset = asset;
            // Apply parameters
            if (parameters is VisualParameters) ApplyParameters((VisualParameters)parameters);
            // Set event key
            _eventKey = eventKey;

            // Branch based on play mode
            if (mode.Equals(EffectManager.PlayMode.Impulse)) InitImpulse();
            else if (mode.Equals(EffectManager.PlayMode.Continuous)) InitContinuous();

            // Play visual
            source.SendEvent(_eventKey);
        }

        #region Initialization
        /// <summary>
        ///     Initializes effect as impulse
        /// </summary>
        private void InitImpulse()
        {
            // Just start the cooldown
            effectTimer.Start();
        }
        /// <summary>
        ///     Initializes effect as continuous
        /// </summary>
        private void InitContinuous()
        {
            // Clear list
            effectTimer.RemoveAllListeners();
            // Ensure source is set up to loop
            effectTimer.OnSuccess += () => effectTimer.Start();
            effectTimer.OnSuccess += () => source.SendEvent(_eventKey);
        }
        #endregion

        #region Parameter Handling
        /// <summary>
        ///     Applies parameters (vp) to component
        /// </summary>
        /// <param name="vp">Input Parameters</param>
        private void ApplyParameters(VisualParameters vp)
        {
            effectTimer.SetRates(vp.length, 1);
        }
        #endregion
        #region Get Methods
        /// <summary>
        ///     Pulls the audio source (source), ensures the value is not null
        /// </summary>
        /// <returns>Audio Source</returns>
        private VisualEffect GetVisualSource()
        {
            if (source == null)
                source = GetComponent<VisualEffect>();
            if (source == null)
                source = gameObject.AddComponent<VisualEffect>();
            return source;
        }
        #endregion
    }
}