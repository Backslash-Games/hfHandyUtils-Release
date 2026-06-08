using UnityEngine;

namespace HFHandyUtils.Physics
{
    /// <summary>
    ///     Defines a cube that checks for collision
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/Hitbox_Cube-34ad086035d381568cbfd30f6659a59e">Documentation</a></br>
    /// </summary>
    [System.Serializable]
    public class Hitbox_Cube : Hitbox
    {
        /// <summary>
        ///     Defines the scale of the Hitbox_Cube
        /// </summary>
        [Header("Cube")]
        public Vector3 size = Vector3.one;
        /// <summary>
        ///     Reference to the Hitbox_Cube euler angles
        /// </summary>
        [Space]
        public Vector3 localEuler = Vector3.zero;

        public Hitbox_Cube(Transform parent) : base(parent) { }

        /// <summary>
        ///     Checks if the hitbox is colliding. Returns state
        /// </summary>
        /// <returns>True when a collision is found</returns>
        public override bool CheckCollision()
        {
            return UnityEngine.Physics.CheckBox(GetWorldPosition(), size / 2f, Quaternion.Euler(localEuler), GetLayerMask());
        }
        /// <summary>
        ///     Gets list of colliding objects. Returns state
        /// </summary>
        /// <param name="collided">List of colliders that the hitbox collided with</param>
        /// <returns>CheckCollision()</returns>
        public override bool GetColliding(out Collider[] collided)
        {
            collided = UnityEngine.Physics.OverlapBox(GetWorldPosition(), size / 2f, Quaternion.Euler(localEuler), GetLayerMask());
            return collided.Length != 0;
        }

        /// <summary>
        ///     Defines how the Hitbox gizmo is drawn
        /// </summary>
        public override void DrawGizmos()
        {
            // Start by setting the color
            SetGizmoColor();
            Gizmos.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), GetWorldPosition(), Quaternion.Euler(localEuler), size);
        }
    }
}
