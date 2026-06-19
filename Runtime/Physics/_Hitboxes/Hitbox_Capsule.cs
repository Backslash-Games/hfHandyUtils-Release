using UnityEditor;
using UnityEngine;

namespace HFHandyUtils.Physics
{
    /// <summary>
    ///     Defines a capsule that checks for collision
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="">Documentation</a></br>
    /// </summary>
    [System.Serializable]
    public class Hitbox_Capsule : Hitbox
    {
        /// <summary>
        ///     Defines the radius of the Hitbox_Capsule
        /// </summary>
        [Header("Capsule")]
        public float radius = 1;
        /// <summary>
        ///     Defines the height of the Hitbox_Capsule
        /// </summary>
        public float height = 1;
        /// <summary>
        ///     Reference to the Hitbox_Cube euler angles
        /// </summary>
        [Space]
        public Vector3 localEuler = Vector3.zero;

        public Hitbox_Capsule(Transform parent) : base(parent) { }

        /// <summary>
        ///     Checks if the hitbox is colliding. Returns state
        /// </summary>
        /// <returns>True when a collision is found</returns>
        public override bool CheckCollision()
        {
            return UnityEngine.Physics.CheckCapsule(GetWorldPosition(), GetWorldEndPosition(), radius);
        }
        /// <summary>
        ///     Gets list of colliding objects. Returns state
        /// </summary>
        /// <param name="collided">List of colliders that the hitbox collided with</param>
        /// <returns>CheckCollision()</returns>
        public override bool GetColliding(out Collider[] collided)
        {
            collided = UnityEngine.Physics.OverlapCapsule(GetWorldPosition(), GetWorldEndPosition(), radius, GetLayerMask());
            return collided.Length != 0;
        }

        /// <summary>
        ///     Defines how the Hitbox gizmo is drawn
        /// </summary>
        public override void DrawGizmos()
        {
            #if UNITY_EDITOR
            // Start by setting the color
            SetGizmoColor();

            // Draw Capsule
            Handles.color = Gizmos.color;

            // -> Get relative values
            Quaternion cAngle = Quaternion.Euler(localEuler);
            Vector3 rUp         = cAngle * Vector3.up;
            Vector3 rForward    = cAngle * Vector3.forward;
            Vector3 rBack       = cAngle * Vector3.back;
            Vector3 rRight      = cAngle * Vector3.right;
            Vector3 rLeft       = cAngle * Vector3.left;

            // -> Bottom semi-sphere
            Handles.DrawWireArc(GetWorldPosition(), rForward, rLeft, 180, radius);
            Handles.DrawWireArc(GetWorldPosition(), rLeft, rBack, 180, radius);
            Handles.DrawWireDisc(GetWorldPosition(), rUp, radius);
            // -> Top semi-sphere
            Handles.DrawWireArc(GetWorldEndPosition(), rForward, rRight, 180, radius);
            Handles.DrawWireArc(GetWorldEndPosition(), rLeft, rForward, 180, radius);
            Handles.DrawWireDisc(GetWorldEndPosition(), rUp, radius);
            // -> Edges
            Handles.DrawLine(   GetWorldPosition() + rForward * radius, GetWorldEndPosition() + rForward * radius );
            Handles.DrawLine(   GetWorldPosition() + rBack * radius,    GetWorldEndPosition() + rBack * radius    );
            Handles.DrawLine(   GetWorldPosition() + rRight * radius,   GetWorldEndPosition() + rRight * radius   );
            Handles.DrawLine(   GetWorldPosition() + rLeft * radius,    GetWorldEndPosition() + rLeft * radius    );

            /*
            // -> Diagonal drawing
            Vector3 forwardRight = new Vector3(0.7f, 0, 0.7f);
            Vector3 forwardLeft = new Vector3(-0.7f, 0, 0.7f);

            Vector3 backRight = new Vector3(0.7f, 0, -0.7f);
            Vector3 backLeft = new Vector3(-0.7f, 0, -0.7f);
            
            Handles.DrawWireArc(GetWorldPosition(), forwardRight, forwardLeft, 180, radius);
            Handles.DrawWireArc(GetWorldPosition(), forwardLeft, backLeft, 180, radius);

            Handles.DrawWireArc(GetWorldEndPosition(), forwardRight, backRight, 180, radius);
            Handles.DrawWireArc(GetWorldEndPosition(), forwardLeft, forwardRight, 180, radius);

            Handles.DrawLine(   GetWorldPosition() + forwardRight * radius,    GetWorldEndPosition() + forwardRight * radius    );
            Handles.DrawLine(   GetWorldPosition() + forwardLeft * radius,     GetWorldEndPosition() + forwardLeft * radius     );
            Handles.DrawLine(   GetWorldPosition() + backRight * radius,       GetWorldEndPosition() + backRight * radius       );
            Handles.DrawLine(   GetWorldPosition() + backLeft * radius,        GetWorldEndPosition() + backLeft * radius        );
            */
            #endif
        }
        private Vector3 GetWorldEndPosition()
        {
            return GetWorldPosition() + (Quaternion.Euler(localEuler) * Vector3.up * height);
        }
    }
}
