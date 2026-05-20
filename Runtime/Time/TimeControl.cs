using System.Collections;
using UnityEngine;

namespace HFHandyUtils.Time
{
    /// <summary>
    ///     Organized method of controlling Unity Time
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/TimeControl-34ad086035d38056ae4df54023b4bca8">Documentation</a></br>
    /// </summary>
    public class TimeControl
    {
        #region Variables
        /// <summary>
        ///     Default time scale on awake
        /// </summary>
        private static float defaultScale = 0;
        /// <summary>
        ///     Reference to parent object
        /// </summary>
        private MonoBehaviour mono = null;
        #endregion

        #region Constructor
        public TimeControl(MonoBehaviour mono)
        {
            // Set the private scale if it hasnt been set
            if (defaultScale == 0)
                defaultScale = UnityEngine.Time.timeScale;
            // Set the mono behaviour
            this.mono = mono;
        }
        #endregion
        #region Time Scale Adjustments
        /// <summary>
        ///     Instantly resets time scale to default
        /// </summary>
        public void ResetScale() { SetScale(defaultScale); }
        /// <summary>
        ///     Sets Unity time scale
        /// </summary>
        /// <param name="amount">New time scale</param>
        public void SetScale(float amount)
        {
            UnityEngine.Time.timeScale = amount;
        }
        /// <summary>
        ///     Sets Unity time scale with a fading reset over a period of time
        /// </summary>
        /// <param name="amount">Initial Amount</param>
        /// <param name="speed">Reset Speed</param>
        public void SetScale_AutoReset_OverTime(float amount, float speed)
        {
            SetScale(amount); // Set the scale right out of the gates
            mono.StartCoroutine(AutoResetScale_OverTime(speed)); // Resets scale over time
        }
        /// <summary>
        ///     Sets Unity time scale with a force reset after delay
        /// </summary>
        /// <param name="amount">Initial Amount</param>
        /// <param name="speed">Reset Speed</param>
        public void SetScale_AutoReset_Delay(float amount, float delay)
        {
            SetScale(amount); // Set the scale right out of the gates
            mono.StartCoroutine(AutoResetScale_Delay(delay)); // Resets scale over time
        }

        /// <summary>
        ///     Logic for fading reset over a period of time
        /// </summary>
        /// <param name="speed">Reset speed</param>
        /// <returns>Wait</returns>
        private IEnumerator AutoResetScale_OverTime(float speed)
        {
            // Move time scale towards default
            while (UnityEngine.Time.timeScale != defaultScale)
            {
                yield return new WaitForEndOfFrame(); // Stalls til end of frame
                UnityEngine.Time.timeScale = Mathf.MoveTowards(UnityEngine.Time.timeScale, defaultScale, UnityEngine.Time.deltaTime * speed * (defaultScale / UnityEngine.Time.timeScale));
            }
        }

        /// <summary>
        ///     Logic for force reset after delay
        /// </summary>
        /// <param name="speed">Reset speed</param>
        /// <returns>Wait</returns>
        private IEnumerator AutoResetScale_Delay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay); // Delays
            UnityEngine.Time.timeScale = defaultScale;
        }
        #endregion

        #region String Methods
        /// <summary>
        ///     Formats string as follows… 
        ///     <br></br> Time Scale: {Time.timeScale}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "";

            output += $"Time Scale: {UnityEngine.Time.timeScale}\n";

            return output;
        }
        #endregion
    }
}