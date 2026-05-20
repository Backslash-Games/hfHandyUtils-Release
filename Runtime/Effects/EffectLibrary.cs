using UnityEngine;

namespace HFHandyUtils.Effects
{
    /// <summary>
    ///     Collection of items that can be played as an effect
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/EffectLibrary-34ad086035d3806b9fd2cbab5c321cef">Documentation</a></br>
    /// </summary>
    /// <typeparam name="TState">Enum State definition. Used to categorize TContent based on enum states.</typeparam>
    /// <typeparam name="TContent">Content stored in the EffectLibrary</typeparam>
    /// <typeparam name="TParams">Effect parameters</typeparam>
    [System.Serializable]
    public class EffectLibrary<TState, TContent, TParams>
        where TState : System.Enum
        where TContent : class
        where TParams : EffectComponent.Parameters
    {
        /// <summary>
        ///     Current state of the effect, when changed it triggers the associated event
        /// </summary>
        [SerializeField] private TParams parameters;
        /// <summary>
        ///     Entries contained within the EffectLibrary
        /// </summary>
        [SerializeField] private EffectLibraryEntry<TState, TContent>[] entries;

        #region Get Methods - Entries
        /// <summary>
        ///     Compares the TContent type with other 
        /// </summary>
        /// <param name="other">Other Type</param>
        /// <returns>True if the types are equal</returns>
        public bool CompareContentType(System.Type other) { return other.Equals(typeof(TContent)); }

        /// <summary>
        ///     Pulls element from entries based on state
        /// </summary>
        /// <param name="state">Enum state</param>
        /// <returns>Content based on state</returns>
        public EffectLibraryEntry<TState, TContent> GetEntryFromState(TState state)
        {
            // Get audio clip of type
            foreach (EffectLibraryEntry<TState, TContent> value in entries)
                if (value.GetState().Equals(state))
                    return value;

            // If state cannot be found
            return null;
        }
        /// <summary>
        ///     Pulls TContent from entries based on state
        /// </summary>
        /// <param name="state">Enum state</param>
        /// <returns>Content based on state</returns>
        public TContent GetContentFromState(TState state)
        {
            // Get audio clip of type
            foreach (EffectLibraryEntry<TState, TContent> value in entries)
                if (value.GetState().Equals(state))
                    return value.GetContent();

            // If state cannot be found
            return default(TContent);
        }
        #endregion
        #region Get Methods - Parameters
        /// <summary>
        ///     Gets parameters
        /// </summary>
        /// <returns>Parameters</returns>
        public TParams GetParams() { return parameters; }
        #endregion
    }
}