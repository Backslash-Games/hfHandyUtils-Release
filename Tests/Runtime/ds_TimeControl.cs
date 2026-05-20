using UnityEngine;
using HFHandyUtils.Time;

namespace HFHandyUtils.DebugScripts
{
    /// <summary>
    ///     Debug script for TimeControl... Should only be used in Unity Inspector
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="">Documentation</a></br>
    /// </summary>
    [AddComponentMenu("HFHandyUtils/Debug/ds_TimeControl")]
    public class ds_TimeControl : MonoBehaviour
    {
        [SerializeField] private TimeControl timeControl = null;
        [SerializeField] private float timeScale = 1;

        // Start is called on the first frame
        private void Start()
        {
            timeControl = new TimeControl(this);
        }
        // Update is called once per frame
        void Update()
        {
            timeControl.SetScale(timeScale);
        }
    }
}