using UnityEngine;

namespace HFHandyUtils.Physics
{
    /// <summary>
    ///     Defines a sphere that checks for collision
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/Hitbox_Sphere-34ad086035d381f3a39ddf2cd2996fa9">Documentation</a></br>
    /// </summary>
    [System.Serializable]
    public class Hitbox_Sphere : Hitbox
    {
        /// <summary>
        ///     Defines the radius of the Hitbox_Sphere
        /// </summary>
        [Header("Sphere")]
        public float radius = 1;

        public Hitbox_Sphere(Transform parent) : base(parent) { }

        /// <summary>
        ///     Checks if the hitbox is colliding. Returns state
        /// </summary>
        /// <returns>True when a collision is found</returns>
        public override bool CheckCollision()
        {
            return UnityEngine.Physics.CheckSphere(GetWorldPosition(), radius, GetLayerMask());
        }
        /// <summary>
        ///     Gets list of colliding objects. Returns state
        /// </summary>
        /// <param name="collided">List of colliders that the hitbox collided with</param>
        /// <returns>CheckCollision()</returns>
        public override bool GetColliding(out Collider[] collided)
        {
            collided = UnityEngine.Physics.OverlapSphere(GetWorldPosition(), radius, GetLayerMask());
            return collided.Length != 0;
        }

        /// <summary>
        ///     Defines how the Hitbox gizmo is drawn
        /// </summary>
        public override void DrawGizmos()
        {
            // Start by setting the color
            SetGizmoColor();
            // Draw the sphere
            Gizmos.DrawSphere(GetWorldPosition(), radius);
        }
    }
}