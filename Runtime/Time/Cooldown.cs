using System.Collections;
using UnityEngine;

namespace HFHandyUtils.Time
{
    /// <summary>
    ///     Organized method of handling cooldown events
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/Cooldown-350d086035d38004935bcd46aac4fdf0">Documentation</a></br>
    /// </summary>
    public class Cooldown
    {
        MonoBehaviour mono = null;

        float basic = 0;
        float reductionRate = 0;
        float timer = 0;

        #region Flags
        bool locked = false;
        bool canceled = false;
        bool paused = false;
        #endregion
        #region Cooldown State Events
        public delegate void CooldownState();
        /// <summary>
        ///     Runs when the cooldown starts
        /// </summary>
        public event CooldownState OnStart;
        /// <summary>
        ///     Runs while cooling down
        /// </summary>
        public event CooldownState OnUpdate;
        /// <summary>
        ///     Runs when the cooldown is paused
        /// </summary>
        public event CooldownState OnPause;
        /// <summary>
        ///     Runs when the cooldown completes without being canceled
        /// </summary>
        public event CooldownState OnSuccess;
        /// <summary>
        ///     Runs when the cooldown is canceled
        /// </summary>
        public event CooldownState OnCancel;
        /// <summary>
        ///     Runs when the cooldown ends in any way
        /// </summary>
        public event CooldownState OnEnd;
        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="mono_parent">Reference to parent MonoBehaviour</param>
        /// <param name="basic">Total Cooldown duration, Unmodified</param>
        /// <param name="reductionRate">Modifier applied to UnityEngine.Time.deltaTime when reducing the time of the Cooldown</param>
        public Cooldown(MonoBehaviour mono_parent, float basic, float reductionRate)
        {
            mono = mono_parent;
            SetRates(basic, reductionRate);
        }

        #region Get Methods
        /// <summary>
        ///     Returns true if the Cooldown is not running
        /// </summary>
        /// <returns>True if its off cooldown</returns>
        public bool Available()
        {
            return timer <= 0 && !Active();
        }

        /// <summary>
        ///     Returns true if the Cooldown is running
        /// </summary>
        /// <returns>Locked state</returns>
        public bool Active()
        {
            return locked;
        }

        /// <summary>
        ///     Gets the percent completion of the Cooldown
        /// </summary>
        /// <returns>A value between 0 and 1 that shows how far through the cooldown is. 1 being finished</returns>
        public float GetPercentComplete()
        {
            return Mathf.Clamp01((basic - timer) / basic);
        }
        #endregion
        #region Set Methods
        /// <summary>
        ///     Sets the rates for the cooldown
        /// </summary>
        /// <param name="basic">Total Cooldown duration, Unmodified</param>
        /// <param name="reductionRate">Modifier applied to UnityEngine.Time.deltaTime when reducing the time of the Cooldown </param>
        public void SetRates(float basic, float reductionRate)
        {
            this.basic = basic;
            this.reductionRate = Mathf.Clamp(reductionRate, 0.01f, float.MaxValue);
        }
        /// <summary>
        ///     Resets the timer for the cooldown
        /// </summary>
        public void ResetTimer()
        {
            // Reset timer
            timer = basic;
        }

        /// <summary>
        ///     Removes all event listeners
        /// </summary>
        public void RemoveAllListeners()
        {
            OnStart = null;
            OnUpdate = null;
            OnPause = null;
            OnSuccess = null;
            OnEnd = null;
            OnCancel = null;
        }
        #endregion

        #region Math
        /// <summary>
        ///     Adds a value to the timer
        /// </summary>
        /// <param name="value">Amount of time added</param>
        public void AddTime(float value)
        {
            timer += value;
        }
        #endregion
        #region Events
        private Coroutine activeRoutine = null;

        /// <summary>
        ///     Starts the Cooldown. 
        ///     <br></br> A delay can be set to start cooldown after a delay period
        /// </summary>
        public void Start(float delay = 0)
        {
            // Check if monobehavour is set properly
            if (mono == null)
                return;
            // Check if the coroutine is locked
            if (Active())
                return;
            // Check if the coroutine is paused, if so pass off to unpause
            if (paused)
            {
                Unpause();
                return;
            }


            // Stops the active routine
            StopActiveRoutine();
            // Reset the cooldown
            ResetTimer();
            // Start the cooldown
            activeRoutine = mono.StartCoroutine(Cooldown_Enum(delay));
        }
        /// <summary>
        ///     Cancels the Cooldown 
        /// </summary>
        public void Cancel()
        {
            // Set flags
            canceled = true;
            OnCancel?.Invoke();
            // Stops the coroutine in its tracks
            timer = 0;

            // Stops the active routine
            StopActiveRoutine();
        }
        /// <summary>
        ///     Pauses the Cooldown 
        /// </summary>
        public void Pause()
        {
            // Set flag
            paused = true;
            // Stops the active routine
            StopActiveRoutine();
            // Run event
            OnPause?.Invoke();
        }
        /// <summary>
        ///     Unpauses the Cooldown with no delay
        /// </summary>
        private void Unpause()
        {
            // Set flag
            paused = false;

            // Continue the cooldown
            activeRoutine = mono.StartCoroutine(Cooldown_Enum());
        }
        #endregion
        #region Coroutine
        /// <summary>
        ///     Asynchronously handles Cooldown logic.
        ///     A delay can be set to start logic after a delay period
        /// </summary>
        /// <param name="delay">Defined delay, runs between flags and on cooldown started</param>
        /// <returns>Wait</returns>
        private IEnumerator Cooldown_Enum(float delay = 0)
        {
            // Lock the coroutine
            locked = true;
            canceled = false;

            // Run delay
            yield return new WaitForSeconds(delay);


            // Invoke event
            OnStart?.Invoke();

            // Start looping the timer
            while (timer > 0)
            {
                // Reduce the timer
                timer -= UnityEngine.Time.deltaTime * reductionRate;
                OnUpdate?.Invoke();
                yield return new WaitForEndOfFrame();
            }

            // Runs only if the coroutine is successful
            if (!canceled)
                OnSuccess?.Invoke();

            // Unlock the coroutine
            locked = false;
            // Runs no matter how the coroutine ends
            OnEnd?.Invoke();
        }


        /// <summary>
        ///     Stops the activeRoutine
        /// </summary>
        private void StopActiveRoutine()
        {
            // Try cancel an active routine
            if (activeRoutine == null)
                return;

            // Stops the routine
            mono.StopCoroutine(activeRoutine);
            // Unlocks the routine
            locked = false;
        }
        #endregion

        #region String Methods
        /// <summary>
        ///     Formats string as follows…
        ///     <br></br> {basic} | {reductionRate} :: { timer}
        ///     <br></br> Locked: { locked}..Canceled:{canceled}..Paused:{ paused}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "";

            output += $"{basic}|{reductionRate} :: {timer}\n";
            output += $"Locked:{locked} .. Canceled:{canceled} .. Paused:{paused}\n";

            return output;
        }
        #endregion
    }
}