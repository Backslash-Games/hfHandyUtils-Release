using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HFHandyUtils.UI
{
    [AddComponentMenu("HFHandyUtils/UI/DraggableComponent")]
    public class DraggableComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        ///     Snaps the draggable component back to its origin when let go
        /// </summary>
        [Tooltip("Snaps the draggable component back to its origin when let go. Lerps if nothing is activated, teleports otherwise")]
        public bool snapToOrigin = false;
        /// <summary>
        ///     Current reset position
        /// </summary>
        [HideInInspector] public Vector3 resetPosition = Vector3.zero;


        /// <summary>
        ///     Flag that checks if the component is held
        /// </summary>
        private bool _held = false;
        /// <summary>
        ///     Current target position
        /// </summary>
        private Vector3 _targetPosition = Vector3.zero;


        /// <summary>
        ///     Tracks event data for click and drag
        /// </summary>
        private static PointerEventData s_EventData = null;
        /// <summary>
        ///     Tracks object for click and drag
        /// </summary>
        private static DraggableComponent s_HeldObject = null;

        /// <summary>
        ///     Movement speed
        /// </summary>
        private static float s_MovementSpeed = 37.5f;


        #region Unity Methods
        private void Awake()
        {
            SetResetPosition(transform.position);
        }
        private void Update()
        {
            if (!_held) return;
            // Click and drag logic
            if (s_EventData != null && s_HeldObject != null) s_HeldObject.SetTargetPosition(s_EventData.position);
        }
        private void LateUpdate()
        {
            LateUpdateMovement();
        }
        #endregion

        #region Interface
        public void OnPointerDown(PointerEventData eventData)
        {
            // Start click and drag
            s_EventData = eventData;
            s_HeldObject = this;
            _held = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Make sure our movement is done
            SetTargetPosition(eventData.position);

            // Stop click and drag
            s_EventData = null;
            s_HeldObject = null;
            _held = false;

            // Run drop logic
            bool activated = OnDrop(eventData);

            // Reset our position
            if (snapToOrigin && !activated) ResetPosition();
            else if(snapToOrigin && activated) Teleport(resetPosition);
            else if(!snapToOrigin) SetResetPosition(transform.position);
        }
        #endregion
        #region Virtuals
        /// <summary>
        ///     Method run when the object is dropped
        /// </summary>
        /// <param name="eventData">Input event data</param>
        /// <returns>True when object is activated</returns>
        public virtual bool OnDrop(PointerEventData eventData)
        {
            return false;
        }
        #endregion

        #region Data
        /// <summary>
        ///     Gets all results based on pointer event data position
        /// </summary>
        /// <param name="eventData">Pointer Event Data</param>
        /// <returns>List of Raycast Results</returns>
        public List<RaycastResult> GetHoveringResults(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results;
        }
        #endregion
        #region Positioning
        /// <summary>
        ///     Lerps movement to target position
        /// </summary>
        private void LateUpdateMovement()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, UnityEngine.Time.deltaTime * s_MovementSpeed);
        }

        /// <summary>
        ///     Sets the target position
        /// </summary>
        /// <param name="targetPosition">New target</param>
        public void SetTargetPosition(Vector3 targetPosition) { _targetPosition = targetPosition; }
        /// <summary>
        ///     Sets the reset position
        /// </summary>
        /// <param name="resetPosition">New reset</param>
        public void SetResetPosition(Vector3 resetPosition) 
        { 
            this.resetPosition = resetPosition;
            SetTargetPosition(this.resetPosition);
        }
        /// <summary>
        ///     Resets the tile position
        /// </summary>
        public void ResetPosition()
        {
            SetTargetPosition(resetPosition);
        }

        /// <summary>
        ///     Teleports the object to a position
        /// </summary>
        /// <param name="position">new position</param>
        public void Teleport(Vector3 position)
        {
            SetTargetPosition(position);
            transform.position = position;
        }
        #endregion
    }
}
