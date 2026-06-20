namespace HFHandyUtils
{
    /// <summary>
    ///     Exposes a method that triggers an object.
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/ITrigger-366d086035d380789458e22178a7afd2">Documentation</a></br>
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        ///     Method that runs logic when the object is triggered
        /// </summary>
        protected void OnTrigger();
    }
}