namespace HFHandyUtils
{
    /// <summary>
    ///     Exposes a method that activates an object.
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/IActivatable-34ad086035d380c491a9f3f8dfa1b62a">Documentation</a></br>
    /// </summary>
    public interface IActivatable
    {
        /// <summary>
        ///     Method that runs logic when the object is activated
        /// </summary>
        protected void OnActivate();

        /// <summary>
        ///     Method that runs logic when the object is deactivated
        /// </summary>
        protected void OnDeactivate();
    }
}
