using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

namespace HFHandyUtils.Data.Props
{
    [AddComponentMenu("HFHandyUtils/Data/Props/Prop")]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Prop : MonoBehaviour
    {
        #region Static Properties
        /// <summary>
        ///     Threshold for edge verticies
        /// </summary>
        private static float _edgeThreshold = 0.05f;
        #endregion
        #region Inspector Visable Properties
        /// <summary>
        ///     Current mesh of the prop
        /// </summary>
        public Mesh mesh = null;

        /// <summary>
        ///     Current material of the prop
        /// </summary>
        public Material material = null;
        #endregion
        #region Inspector Hidden Properties
        /// <summary>
        ///     Tracks if the prop mesh has been initialized
        /// </summary>
        public bool meshInitialized = false;

        /// <summary>
        ///     Flag that allows mesh to update when modifying the transform
        /// </summary>
        public bool transformMeshUpdate = false;

        /// <summary>
        ///     Extents in which the prop resides
        /// </summary>
        public Bounds boundingBox = new Bounds();
        /// <summary>
        ///     Prop volume
        /// </summary>
        public float volume = 0;

        /// <summary>
        ///     Managed mesh filter. Do not reference
        /// </summary>
        private MeshFilter m_MeshFilter = null;
        /// <summary>
        ///     Prop Mesh Filter
        /// </summary>
        public MeshFilter meshFilter
        {
            get
            {
                if (m_MeshFilter == null)
                    m_MeshFilter = GetComponent<MeshFilter>();
                return m_MeshFilter;
            }
        }

        /// <summary>
        ///     Managed mesh renderer. Do not reference
        /// </summary>
        private MeshRenderer m_MeshRenderer = null;
        /// <summary>
        ///     Prop Mesh Renderer
        /// </summary>
        public MeshRenderer meshRenderer
        {
            get
            {
                if (m_MeshRenderer == null)
                    m_MeshRenderer = GetComponent<MeshRenderer>();
                return m_MeshRenderer;
            }
        }

        /// <summary>
        ///     A list of all verticies used in the simulation
        ///     [!] Gets cleaned after computation
        /// </summary>
        public Vector3[] simulatedVerticies;
        /// <summary>
        ///     Connection points that are listed around the edge of a prop, snap to vertex
        ///     [!] Gets cleaned after computation (unless sys. def. symbol - HFHANDY_DEVELOPMENT)
        /// </summary>
        public Vector3[] edgeVerticies;
        /// <summary>
        ///     Connection points that are listed around the edge of a prop, snap to face
        ///     <br></br>
        ///     <br>[0] - Top</br>
        ///     <br>[1] - Bottom</br>
        ///     <br>[2] - Left</br>
        ///     <br>[3] - Right</br>
        ///     <br>[4] - Forward</br>
        ///     <br>[5] - Backwards</br>
        /// </summary>
        public Vector3[] faceVerticies = new Vector3[6];
        #endregion
        #region Debug Properties
        #if HFHANDY_DEVELOPMENT
        /// <summary>
        ///     Average tracker for tick prop mesh. Disabled by HFHANDY_DEVELOPMENT
        /// </summary>
        private HFLogger.AverageTracker _tickMeshAverage = new HFLogger.AverageTracker();
        /// <summary>
        ///     Tracks the time it takes to tick prop mesh. Disabled by HFHANDY_DEVELOPMENT
        /// </summary>
        private System.Diagnostics.Stopwatch _tickStopwatch = new System.Diagnostics.Stopwatch();
        #endif
        #endregion

        #region General Events
        /// <summary>
        ///     Updates every component of the Prop
        /// </summary>
        public void TickAll()
        {
            TickMesh("Tick All");
        }
        #endregion
        #region Mesh Events        
        /// <summary>
        ///     Updates all mesh related components
        /// </summary>
        /// <param name="source">Call source</param>
        /// <param name="requireEditor">Flag that requries game to be running in editor to update</param>
        public void TickMesh(string source = "Unknown Source", bool requireEditor = true)
        {
            #if HFHANDY_DEVELOPMENT
            _tickStopwatch.Restart();
            #endif

            // Check if the application is running
            if (requireEditor && Application.isPlaying)
                return;

            // Set Mesh
            SetMeshComponents();

            // Error checking
            if (mesh == null)
            {
                HFLogger.LogError($"Please set a mesh for Prop '{name}' to function properly");
                return;
            }
            if (mesh.vertices.Length <= 0)
            {
                HFLogger.LogError($"Prop '{name}' tried setting bounds on a mesh with no verticies");
                return;
            }

            // Execution order
            SetSimulatedVerticies();
            SetBounds();
            SetEdgeVerticies();
            SetFaceVerticies();
            SetVolume();

            // Clean up information from computation
            CleanUp();

            #if HFHANDY_DEVELOPMENT
            _tickStopwatch.Stop();
            HFLogger.LogTime($"{name} Tick ({source})", _tickStopwatch.ElapsedTicks, _tickMeshAverage);
            #endif

            meshInitialized = true;
        }
        /// <summary>
        ///     Executes bound updates when modifying transform
        /// </summary>
        /// <param name="source">Call source</param>
        /// <param name="requireEditor">Flag that requries game to be running in editor to update</param>
        /// <returns>True if successful</returns>
        public bool TickMesh_Transform(string source = "Unknown Source", bool requireEditor = true)
        {
            if (!transformMeshUpdate) return false;
            TickMesh(source, requireEditor);
            return true;
        }

        /// <summary>
        ///     Sets all mesh components to be current with 'mesh'
        /// </summary>
        public void SetMeshComponents()
        {
            // Check if the mesh is initialized
            if (!meshInitialized && mesh == null) mesh = meshFilter.sharedMesh;
            if (!meshInitialized && material == null) material = meshRenderer.sharedMaterial;

            #if HFHANDY_DEVELOPMENT
            // Reset the average tracker if our mesh has changed
            if (meshFilter.sharedMesh != mesh)
            {
                _tickMeshAverage.Reset();
                HFLogger.Log("Reset timer average");
            }
            #endif
            
            // Set the mesh
            meshFilter.mesh = mesh;
            // Set the material
            meshRenderer.material = material;
        }

        /// <summary>
        ///     Creates a list of the simulated verticies
        /// </summary>
        public void SetSimulatedVerticies()
        {
            // Establish collection
            List<Vector3> cSimulatedVerticies = new List<Vector3>();

            // Get a list of all the contained mesh filters in the object
            MeshFilter[] cFilters = GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter cFilter in cFilters)
                cSimulatedVerticies.AddRange(Mathh.GetVerticiesFromMesh(cFilter.transform, cFilter.sharedMesh));

            // Set simulated verticies
            simulatedVerticies = cSimulatedVerticies.ToArray();
        }

        /// <summary>
        ///     Sets the bounds of the prop based on vertices
        /// </summary>
        private void SetBounds()
        {
            // Reset bounds
            boundingBox = new Bounds();

            // Roll through vertices and extend bounds to encompass
            for(int i = 0; i < simulatedVerticies.Length; i++)
            {
                // Get the vertex
                Vector3 vertex = simulatedVerticies[i];

                // Check for instantiation versus encapulation
                if (i == 0)
                    boundingBox.center = vertex;
                else
                    boundingBox.Encapsulate(vertex);
            }

            // Finalize bounding box
            boundingBox.size = Mathh.Vector_Abs(boundingBox.size);
        }

        /// <summary>
        ///     Sets the vertex connections that are contained between meshBounds and edgeThresholdBounds
        /// </summary>
        private void SetEdgeVerticies()
        {
            // Create simulated bounds for edge verticies
            Bounds edgeBounds = new Bounds(boundingBox.center, boundingBox.size - (transform.localScale * _edgeThreshold));
            // Create a list for found points
            List<Vector3> foundEdges = new List<Vector3>();

            // Roll through vertices and check if they are outside of the edge bounds
            for (int i = 0; i < simulatedVerticies.Length; i++)
            {
                // Get the vertex
                Vector3 vertex = simulatedVerticies[i];
                // Check point
                if (!edgeBounds.Contains(vertex))
                    foundEdges.Add(vertex);
            }

            // Write list
            edgeVerticies = foundEdges.ToArray();
        }

        /// <summary>
        ///     Sets face connections that are defined along each face
        /// </summary>
        private void SetFaceVerticies()
        {
            // -> Top
            SetFaceVertex(0, Vector3.up, boundingBox.size.y);
            // -> Bottom
            SetFaceVertex(1, Vector3.down, boundingBox.size.y);

            // -> Left
            SetFaceVertex(2, Vector3.left, boundingBox.size.x);
            // -> Right
            SetFaceVertex(3, Vector3.right, boundingBox.size.x);

            // -> Forward
            SetFaceVertex(4, Vector3.forward, boundingBox.size.z);
            // -> Backward
            SetFaceVertex(5, Vector3.back, boundingBox.size.z);
        }
        /// <summary>
        ///     Calculates face-bounds and stores averaged vertex data
        /// </summary>
        /// <param name="index">Face vertices index</param>
        /// <param name="direction">Target direction</param>
        /// <param name="targetSize">Bounding edge size</param>
        private void SetFaceVertex(int index, Vector3 direction, float targetSize)
        {
            // Check face verticies bound
            if (faceVerticies.Length != 6)
                faceVerticies = new Vector3[6];

            // Establish parameters for calculations
            float boundSize = _edgeThreshold * 2;
            float boundParameter = (0.5f * (targetSize - boundSize)) + boundSize / 2f;

            // Build face verticies
            Bounds cBounds = new Bounds();
            cBounds.center = boundingBox.center + (direction * boundParameter);
            cBounds.size = new Vector3(direction.x == 0 ? boundingBox.size.x + _edgeThreshold : boundSize, direction.y == 0 ? boundingBox.size.y + _edgeThreshold : boundSize, direction.z == 0 ? boundingBox.size.z + _edgeThreshold : boundSize);
            faceVerticies[index] = Mathh.GetInBoundsCollectionAverage(edgeVerticies, cBounds);
        }
        /// <summary>
        ///     Definition to calculate face-bound gizmos. Costly. Only used in development
        /// </summary>
        /// <param name="index">Face vertices index</param>
        /// <param name="direction">Target direction</param>
        /// <param name="targetSize">Bounding edge size</param>
        private void GizmoFaceBounds(int index, Vector3 direction, float targetSize)
        {
            float boundSize = _edgeThreshold * 2;
            float boundParameter = (0.5f * (targetSize - boundSize)) + boundSize / 2f;

            Bounds cBounds = new Bounds();
            cBounds.center = boundingBox.center + (direction * boundParameter);
            cBounds.size = new Vector3(direction.x == 0 ? boundingBox.size.x + _edgeThreshold : boundSize, direction.y == 0 ? boundingBox.size.y + _edgeThreshold : boundSize, direction.z == 0 ? boundingBox.size.z + _edgeThreshold : boundSize);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cBounds.center, cBounds.size);
        }
        
        /// <summary>
        ///     Sets the volume of the prop
        /// </summary>
        private void SetVolume()
        {
            volume = Mathh.GetVectorVolume(boundingBox.size);
        }
        
        /// <summary>
        ///     Cleans up extra variables to minimize impact on memory
        /// </summary>
        private void CleanUp()
        {
            // Clear out simulated vertex array
            simulatedVerticies = new Vector3[0];
            #if !HFHANDY_DEVELOPMENT
            // Clear out face vertex array
            edgeVerticies = new Vector3[0];
            #endif
        }
        #endregion
        #region Prop Interactions
        /// <summary>
        ///     Connects this prop to the nearest prop
        /// </summary>
        public void ConnectToNearest()
        {
            // Find nearest
            ConnectToNearest(FindObjectsByType<Prop>(FindObjectsSortMode.None));
        }
        /// <summary>
        ///     Connects this prop to the nearest prop
        /// </summary>
        public void ConnectToNearest(Prop[] props)
        {
            // Check if length is 1
            if (props.Length == 1)
                return;

            // Find nearest
            Prop clstProp = Mathh.GetClosestObject(props, transform.position);
            ConnectToOther(clstProp);
        }
        /// <summary>
        ///     Connects this prop to another prop
        /// </summary>
        /// <param name="other">Input prop - Other</param>
        public void ConnectToOther(Prop other)
        {
            // Pull direction
            Vector3 co_direction = Mathh.SnapToRightAngle(other.transform.position - transform.position); // CURRENT -> OTHER
            // Pull face indexes
            int cFaceIndex = DirectionToIndex(co_direction); // CURRENT

            // Move prop
            transform.position = other.GetOpposingFace_Point(cFaceIndex) + (boundingBox.center - faceVerticies[cFaceIndex]);
        }
        /// <summary>
        ///     Nudges the prop in a direction by extents
        /// </summary>
        /// <param name="direction">Direction</param>
        public void Nudge(Vector3 direction)
        {
            Vector3 nudge = Mathh.MultiplyVectorComponents(direction, boundingBox.extents);
            nudge = Mathh.ClampVectorComponents(nudge, -boundingBox.extents, boundingBox.extents);
            transform.position += nudge;

            #if HFHANDY_DEVELOPMENT
            HFLogger.Log($"Nudged Prop {name} by {nudge} || Direction: {direction} [{direction.normalized}] -- Extents: {boundingBox.extents}");
            #endif
        }
        #endregion

        #region Vertex Data
        /// <summary>
        ///     Pulls the opposing face position
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Vector3 GetOpposingFace_Point(int input)
        {
            try { return faceVerticies[GetOpposingFace_Index(input)]; }
            catch (Exception e) { throw e; }
        }
        #endregion
        #region Vertex Data - Static
        /// <summary>
        ///     Takes a direction and converts it into a face index
        /// </summary>
        /// <param name="direction">Input direction</param>
        /// <returns>Face index</returns>
        public static int DirectionToIndex(Vector3 direction)
        {
            if(direction == Vector3.up) return 0; // TOP
            if(direction == Vector3.down) return 1; // BOTTOM

            if (direction == Vector3.left) return 2; // LEFT
            if (direction == Vector3.right) return 3; // RIGHT

            if (direction == Vector3.forward) return 4; // FORWARD
            if (direction == Vector3.back) return 5; // BACKWARDS

            return -1; // ERROR
        }
        /// <summary>
        ///     Pulls the opposing face index
        /// </summary>
        /// <param name="input">Input index</param>
        /// <returns>Opposing index</returns>
        public static int GetOpposingFace_Index(int input)
        {
            switch (input)
            {
                case 0: return 1; // TOP => BOTTOM
                case 1: return 0; // BOTTOM => TOP

                case 2: return 3; // LEFT => RIGHT
                case 3: return 2; // RIGHT => LEFT

                case 4: return 5; // FORWARD => BACKWARDS
                case 5: return 4; // BACKWARDS => FORWARD

                default : return -1; // ERROR
            }
        }
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            // Return if we havent initialized
            if (!meshInitialized) return;

            // Draw bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);
            // Draw bound center
            Gizmos.DrawSphere(boundingBox.center, 0.1f);



#if HFHANDY_DEVELOPMENT
            // Draw edge bounds
            Gizmos.color = Color.red;
            Bounds edgeBounds = new Bounds(boundingBox.center, boundingBox.size - (Vector3.one * _edgeThreshold));
            Gizmos.DrawWireCube(edgeBounds.center, edgeBounds.size);
            // Draw bound center
            Gizmos.DrawSphere(edgeBounds.center, 0.1f);

            // Draw edge verticies
            for(int i = 0; i < edgeVerticies.Length; i++)
            {
                //Gizmos.color = Color.Lerp(Color.red, Color.orange, (float)i / (edgeVerticies.Length - 1));
                Gizmos.DrawWireSphere(edgeVerticies[i], 0.025f * transform.localScale.magnitude);
            }

            // -> Top
            GizmoFaceBounds(0, Vector3.up, boundingBox.size.y);
            // -> Bottom
            GizmoFaceBounds(1, Vector3.down, boundingBox.size.y);

            // -> Left
            GizmoFaceBounds(2, Vector3.left, boundingBox.size.x);
            // -> Right
            GizmoFaceBounds(3, Vector3.right, boundingBox.size.x);

            // -> Forward
            GizmoFaceBounds(4, Vector3.forward, boundingBox.size.z);
            // -> Backward
            GizmoFaceBounds(5, Vector3.back, boundingBox.size.z);
#endif



            // Draw face verticies
            for (int i = 0; i < faceVerticies.Length; i++)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.blue, (float)i / (faceVerticies.Length - 1));
                Gizmos.DrawWireSphere(faceVerticies[i], 0.1f * transform.localScale.magnitude);
            }
        }
        #endregion
    }
}
