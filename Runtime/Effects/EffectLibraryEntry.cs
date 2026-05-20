using UnityEngine;


namespace HFHandyUtils.Effects
{
    /// <summary>
    ///     Entry contained within the EffectLibrary
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/EffectLibraryEntry-34ad086035d38098b1d9e44f97fa9c5f">Documentation</a></br>
    /// </summary>
    /// <typeparam name="TKey">Enum state key definition</typeparam>
    /// <typeparam name="TValue">Content associated with TKey</typeparam>
    [System.Serializable]
    public class EffectLibraryEntry<TKey, TValue>
        where TKey : System.Enum
        where TValue : class
    {
        /// <summary>
        ///     Enum state key definition
        /// </summary>
        [SerializeField] private TKey state;
        /// <summary>
        ///     Content associated with state
        /// </summary>
        [SerializeField] private TValue content;

        /// <summary>
        ///     Key used for effects that require ‘event strings’ to trigger
        /// </summary>
        [Space] [SerializeField] private string eventKey;

        #region Constructor
        public EffectLibraryEntry(TKey key, TValue value, string eventKey)
        {
            state = key;
            content = value;
            this.eventKey = eventKey;
        }
        public EffectLibraryEntry(TKey key, TValue value) : this(key, value, "") { }
        #endregion

        #region Get Methods
        /// <summary>
        ///     Gets state
        /// </summary>
        /// <returns>State</returns>
        public TKey GetState() { return state; }
        /// <summary>
        ///     Gets content
        /// </summary>
        /// <returns>Content</returns>
        public TValue GetContent() { return content; }
        /// <summary>
        ///     Gets eventKey
        /// </summary>
        /// <returns>Event Key</returns>
        public string GetEventKey() { return eventKey; }
        #endregion
        #region String Handling
        /// <summary>
        ///     Formats string as follows… {state} : {content} -> { eventKey}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{state} : {content} -> {eventKey}";
        }
        #endregion
    }
}