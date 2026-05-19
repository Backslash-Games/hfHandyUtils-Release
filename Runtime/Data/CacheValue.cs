namespace HFHandyUtils.Data
{
    [System.Serializable]
    public class CacheValue<T>
    {
        public T value;
        public bool isSet = false;

        /// <summary>
        ///     Sets the value
        /// </summary>
        /// <param name="value">New value</param>
        public void SetValue(T value, bool force = false)
        {
            // Warn about a reset
            if (isSet)
            {
                UnityEngine.Debug.LogWarning("Trying to override cached value");
                if (!force)
                    return;
            }

            // Set Data
            this.value = value;
            isSet = true;
        }

        /// <summary>
        ///     Resets cache data to allow an override
        /// </summary>
        public void Reset()
        {
            isSet = false;
        }
    }
}
