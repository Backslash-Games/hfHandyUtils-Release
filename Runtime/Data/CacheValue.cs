namespace HFHandyUtils.Data
{
    /// <summary>
    ///     Data type that caches data
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/CacheValue-354d086035d3803c8d7df65de103b73a">Documentation</a></br>
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    [System.Serializable]
    public class CacheValue<T>
    {
        /// <summary>
        ///     Value stored in cache
        /// </summary>
        public T value;
        /// <summary>
        ///     Flag; True when data is already set
        /// </summary>
        public bool isSet = false;

        /// <summary>
        ///     Sets the cached value. Marks isSet as true
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
