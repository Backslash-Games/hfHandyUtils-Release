using UnityEngine;

namespace HFHandyUtils.Graphical
{
    /// <summary>
    ///     Positions an object towards a target
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/Billboard-34ad086035d38067802bf43f27425dab">Documentation</a></br>
    /// </summary>
    [AddComponentMenu("HFHandyUtils/Graphical/Billboard")]
    public class Billboard : MonoBehaviour
    {
        /// <summary>
        ///     Flag; dictates if the billboard is active
        /// </summary>
        [SerializeField] private bool active = true;
        /// <summary>
        ///     Target the billboard looks at
        /// </summary>
        [SerializeField] private Transform target = null;

        // Update is called once per frame
        void Update() { UpdateBillboard(); }



        /// <summary>
        ///     Updates the billboard. Called automatically in Unity Update
        /// </summary>
        private void UpdateBillboard()
        {
            // Check if we are billboarding
            if (!active)
                return;
            // Check if target is null
            if (target == null)
                return;

            // Make canvas object face the main camera
            target.LookAt(Camera.main.transform, Vector3.down);
        }



        /// <summary>
        ///     Sets up all components of the billboard
        /// </summary>
        /// <param name="state">Initial State</param>
        /// <param name="target">Initial Target</param>
        public void Initialize(bool state, Transform target) { SetState(state); SetTarget(target); }
        /// <summary>
        ///     Sets the active state
        /// </summary>
        /// <param name="state">New state</param>
        public void SetState(bool state) { active = state; }
        /// <summary>
        ///     Sets the active target
        /// </summary>
        /// <param name="state">New target</param>
        public void SetTarget(Transform target) { this.target = target; }
    }
}