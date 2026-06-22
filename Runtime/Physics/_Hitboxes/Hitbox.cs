using UnityEngine;

namespace HFHandyUtils.Physics
{
    /// <summary>
    ///     Defines a region that checks for collision
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/Hitbox-34ad086035d380e183f7f0bf1048bedf">Documentation</a></br>
    /// </summary>
    [System.Serializable]
    public class Hitbox
    {
        /// <summary>
        ///     Defines the parent of the Hitbox
        /// </summary>
        [SerializeField] private Transform parent;

        /// <summary>
        ///     Reference to the current collision state
        /// </summary>
        [Space][SerializeField] private bool state = false;
        /// <summary>
        ///     Defines layers the Hitbox can collide with
        /// </summary>
        [SerializeField] private LayerMask mask = 0;

        /// <summary>
        ///     Defines the offset of the Hitbox
        /// </summary>
        [Space][SerializeField] private Vector3 offset;

        public Hitbox(Transform parent) { this.parent = parent; }

        /// <summary>
        ///     Runs all update logic
        /// </summary>
        public void Tick()
        {
            TickState(); // Update the state
        }

        #region State Handling
        /// <summary>
        ///     Updates the collision state
        /// </summary>
        private void TickState() { state = CheckCollision(); }
        /// <summary>
        ///     Gets the current collision state
        /// </summary>
        /// <returns>Collsision state</returns>
        public bool GetState() { return state; }
        #endregion
        #region Collision Handling
        /// <summary>
        ///     Checks if the hitbox is colliding. Returns state
        /// </summary>
        /// <returns>True when a collision is found</returns>
        public virtual bool CheckCollision() { return false; }
        /// <summary>
        ///     Gets list of colliding objects. Returns state
        /// </summary>
        /// <param name="collided">List of colliders that the hitbox collided with</param>
        /// <returns>CheckCollision()</returns>
        public virtual bool GetColliding(out Collider[] collided) { collided = new Collider[0]; return CheckCollision(); }


        /// <summary>
        ///     Gets the current layer mask
        /// </summary>
        /// <returns>Layer mask</returns>
        public LayerMask GetLayerMask() { return mask; }
        /// <summary>
        ///     Sets the current layer mask
        /// </summary>
        /// <param name="layerMask">New Mask</param>
        public void SetLayerMask(LayerMask layerMask) { mask = layerMask; }
        #endregion

        #region Transform
        /// <summary>
        ///     Gets the current parent 
        /// </summary>
        /// <returns>Current parent</returns>
        public Transform GetParent() { return parent; }
        #endregion
        #region Position
        /// <summary>
        ///     Gets the center of the Hitbox in world space
        /// </summary>
        /// <returns>World position</returns>
        public Vector3 GetWorldPosition() { return GetOffset() + GetParent().position; }
        /// <summary>
        ///     Gets the offset of the Hitbox in local space
        /// </summary>
        /// <returns>Offset</returns>
        public Vector3 GetOffset() { return offset; }
        /// <summary>
        ///     Sets the offset of the Hitbox in local space
        /// </summary>
        /// <param name="input">Input Offset</param>
        public void SetOffset(Vector3 input) { offset = input; }
        #endregion

        #region Debug
        /// <summary>
        ///     Transparency of the drawn gizmo
        /// </summary>
        private static readonly float gizmo_alpha = 0.5f;
        /// <summary>
        ///     Tint of the drawn gizmo
        /// </summary>
        private static readonly UnityEngine.Color[] gizmo_colors = new UnityEngine.Color[2] { new UnityEngine.Color(0, 1, 0, gizmo_alpha), new UnityEngine.Color(1, 0, 0, gizmo_alpha) };
        
        
        /// <summary>
        ///     Defines how the Hitbox gizmo is drawn
        /// </summary>
        public virtual void DrawGizmos() { }



        /// <summary>
        ///     Updates Gizmo.Color based on state
        /// </summary>
        public void SetGizmoColor() { SetGizmoColor(GetState()); }
        /// <summary>
        ///     Updates Gizmo.Color based on value
        /// </summary>
        /// <param name="value">Input</param>
        public void SetGizmoColor(bool value) { Gizmos.color = value ? gizmo_colors[0] : gizmo_colors[1]; }
        #endregion
    }
}