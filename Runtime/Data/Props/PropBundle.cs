using HFHandyUtils.Data.Heatmaps;
using System.Collections.Generic;
using UnityEngine;

namespace HFHandyUtils.Data.Props
{
    [AddComponentMenu("HFHandyUtils/Data/Props/PropBundle")]
    public class PropBundle : MonoBehaviour
    {
        /// <summary>
        ///     List of spawnable props
        /// </summary>
        public Prop[] spawnableProps = new Prop[0];
        /// <summary>
        ///     List of spawned objects
        /// </summary>
        public List<Prop> spawnedProps = new List<Prop>();
        /// <summary>
        ///     Amount of props spawned during build
        /// </summary>
        public int spawnCount = 3;

        public enum RotationSettings { None, X, Y, Z, XY, XZ, YZ, ALL };
        /// <summary>
        ///     Settings that control how props are rotated
        /// </summary>
        public RotationSettings rotationSettings = RotationSettings.None;
        public enum SpawnSettings { Random, Volume };
        /// <summary>
        ///     Settings that control how props are spawned
        /// </summary>
        public SpawnSettings spawnSettings = SpawnSettings.Random;

        /// <summary>
        ///     Prop Bundle bounds
        /// </summary>
        public Bounds bounds;
        #region Heatmap
        /// <summary>
        ///     Layer mask used to determine what meshes are considered when generating the heatmap
        /// </summary>
        [SerializeField] private LayerMask heatmapMeshMask;
        /// <summary>
        ///     Heatmap used for dynamic methods
        /// </summary>
        [SerializeField] private Heatmap heatmap;

        public float heatmapPropScaleBonus = 1f;
        public float propThreshold = 0.7f;
        #endregion

        #region Vertex Assesment
        private MeshFilter[] relevantFilters = new MeshFilter[0];
        private Vector3[] relevantVerticies = new Vector3[0];
        private Vector3[] relevantNormals = new Vector3[0];
        private List<Vector3> normalTypes = new List<Vector3>();
        #endregion

        #region Sequencing
        /// <summary>
        ///     Builds the bundle
        /// </summary>
        public void BuildBundle()
        {
            // Destroy all objects that have been spawned
            DestroyAllSpawned();
            ArrangeDynamicStack();
        }
        public void Reset()
        {
            DestroyAllSpawned();

            heatmap.Reset();

            relevantFilters = new MeshFilter[0];
            relevantVerticies = new Vector3[0];
            relevantNormals = new Vector3[0];
            normalTypes.Clear();
        }
        /// <summary>
        ///     Destroy all spawned objects
        /// </summary>
        public void DestroyAllSpawned()
        {
            foreach (Prop prop in spawnedProps)
            {
                #if UNITY_EDITOR
                DestroyImmediate(prop.gameObject);
                #else
                Destroy(prop.gameObject);
                #endif
            }
            spawnedProps.Clear();
        }
        /// <summary>
        ///     Destroy a spawned prop
        /// </summary>
        /// <param name="prop">Input prop</param>
        public void DestroySpawned(Prop prop)
        {
            spawnedProps.Remove(prop);

            #if UNITY_EDITOR
            DestroyImmediate(prop.gameObject);
            #else
            Destroy(prop.gameObject);
            #endif

        }
        #endregion

        #region Pillar Methods
        /// <summary>
        ///     Builds a pillar of props
        /// </summary>
        private void ArrangePillar()
        {
            // Spawn all objects on self
            for (int i = 0; i < spawnCount; i++)
            {
                // Spawn an object
                spawnedProps.Add(Instantiate(GetRandomSpawnableProp(), transform));
            }

            // Go through and organize objects
            spawnedProps[0].TickAll();
            for (int i = 1; i < spawnCount; i++)
            {
                Prop cProp = spawnedProps[i];
                Prop oProp = spawnedProps[i - 1];

                spawnedProps[i].transform.rotation = UnityEngine.Random.rotation;
                spawnedProps[i].TickMesh();

                cProp.transform.position = oProp.transform.position + (Vector3.up * oProp.boundingBox.size.y);
                cProp.ConnectToOther(oProp);
                cProp.TickMesh();
            }
        }
        #endregion
        #region Dynamic Stacking Methods
        public void ArrangeDynamicStack()
        {
            // Setup
            AssessFilters();
            AssessVerticies();
            BuildHeatmap();

            // Placement
            BuildProps_DynamicStack();
        }
        /// <summary>
        ///     Pull all relevant mesh filters and store information
        /// </summary>
        public void AssessFilters()
        {
            // Get all meshes in the scene
            List<MeshFilter> sceneFilters = new List<MeshFilter>(FindObjectsByType<MeshFilter>(FindObjectsSortMode.None));

            // Prune list
            for (int i = 0; i < sceneFilters.Count; i++)
            {
                // Check if object is contained in layer mask
                if ((heatmapMeshMask & (1 << sceneFilters[i].gameObject.layer)) == 0)
                {
                    sceneFilters.RemoveAt(i);
                    i--;
                }
            }

            // Move into relevant filter list
            relevantFilters = sceneFilters.ToArray();
        }
        /// <summary>
        ///     Pull all relevant verticies and store information
        /// </summary>
        public void AssessVerticies()
        {
            List<Vector3> scenePositions = new List<Vector3>();
            List<Vector3> sceneNormals = new List<Vector3>();
            normalTypes.Clear();

            // Roll through all verticies in relevant filters and find relevant positions
            foreach (MeshFilter filter in relevantFilters)
            {
                Vector3[] cVerticies = Mathh.GetVerticiesFromMesh(filter.transform, filter.sharedMesh);
                Vector3[] cNormals = filter.sharedMesh.normals;
                for (int i = 0; i < cVerticies.Length; i++)
                {
                    if (bounds.Contains(cVerticies[i]))
                    {
                        scenePositions.Add(cVerticies[i]);
                        
                        // Handle Normals
                        Vector3 normal = filter.transform.rotation * cNormals[i];
                        sceneNormals.Add(normal);
                        // -> Check if we have found a new (valid) normal type
                        if(Vector3.Dot(normal, Vector3.up) >= -0.01f && !normalTypes.Contains(normal))
                            normalTypes.Add(normal);
                    }
                }
            }

            // Move into relevant verticies list
            relevantVerticies = scenePositions.ToArray();
            relevantNormals = sceneNormals.ToArray();
        }
        /// <summary>
        ///     Builds the heatmap
        /// </summary>
        public void BuildHeatmap()
        {
            // Reset the heatmap
            heatmap.Reset();

            // Calculate vertex heat
            float vertexHeat = 1f / normalTypes.Count;
            // Add heatmap values
            for(int i = 0; i < relevantVerticies.Length; i++)
            {
                float cHeat = vertexHeat + (Vector3.Dot(relevantNormals[i], Vector3.up) * vertexHeat * 0.5f);
                heatmap.AddValue(relevantVerticies[i], vertexHeat, true);
            }
            // Add corners
            heatmap.AddValues(Mathh.GetBoundCorners(heatmap.bounds));

            // Isolate points
            heatmap.IsolateCorners();
            heatmap.IsolateSignificant();
        }
        
        /// <summary>
        ///     Places props in bounds
        /// </summary>
        public void BuildProps_DynamicStack()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                // Check if we have run out of values
                if (!heatmap.HasValues())
                    return;
                // Grab the hottest
                MapValue hottestValue = heatmap.GetHottestValue();
                // Check if our value doesn't surpass our ignore threshold
                if (propThreshold >= hottestValue.value)
                    return;
                // Check if our placement was a success
                if (!PlaceProp_DynamicStack(hottestValue))
                    i--;
            }
        }
        private bool PlaceProp_DynamicStack(MapValue targetValue)
        {
            // Place a prop and track
            Prop placedProp = Instantiate(GetProp(spawnSettings, targetValue), transform);
            spawnedProps.Add(placedProp);

            // Check for a rotation
            placedProp.transform.rotation = EvaluateRotationSettings();
            placedProp.TickAll();

            // Get the hottest value
            placedProp.transform.position = targetValue.point;

            // Nudge the prop into the right place
            placedProp.Nudge(bounds.center - placedProp.transform.position);
            // -> Snap to nearest
            float verticalPosition = placedProp.transform.position.y;
            placedProp.ConnectToNearest(spawnedProps.ToArray());
            if(verticalPosition < placedProp.transform.position.y)
                placedProp.transform.position = new Vector3(placedProp.transform.position.x, verticalPosition, placedProp.transform.position.z);
            // -> Update the prop
            placedProp.TickAll();
            // -> Remove value from map
            heatmap.RemoveValue(targetValue);

            // Check if the placement was successful
            Bounds simulationBounds = placedProp.boundingBox;
            simulationBounds.size = simulationBounds.size * 0.975f;
            for (int i = 0; i < spawnedProps.Count - 1; i++)
            {
                // Check for an intersection
                if (spawnedProps[i].boundingBox.Intersects(simulationBounds))
                {
                    DestroySpawned(placedProp);
                    return false;
                }
            }
            // -> Check for out of bounds
            Vector3[] corners = Mathh.GetBoundCorners(placedProp.boundingBox);
            foreach (Vector3 corner in corners)
            {
                if(!bounds.Contains(corner))
                {
                    DestroySpawned(placedProp);
                    return false;
                }
            }

            // When valid finalize prop
            // -> Add corners to map
            heatmap.AddEvaluations(corners);
            // -> Add connection points to map
            //heatmap.AddEvaluations(placedProp.faceVerticies);

            // Mark as a success
            return true;
        }
        #endregion

        #region Get Methods
        /// <summary>
        ///     Spawns a prop
        /// </summary>
        /// <param name="spawnSettings">Input settings</param>
        /// <param name="heatmapPosition">Input heatmap position</param>
        /// <returns>Prop</returns>
        public Prop GetProp(SpawnSettings spawnSettings, MapValue heatmapPosition)
        {
            switch (spawnSettings)
            {
                case SpawnSettings.Volume:
                    return GetPropByVolume(heatmapPosition);

                default:
                    return GetRandomSpawnableProp();
            }
        }

        /// <summary>
        ///     Pulls a random prop
        /// </summary>
        /// <returns>Random prop</returns>
        private Prop GetRandomSpawnableProp()
        {
            return spawnableProps[UnityEngine.Random.Range(0, spawnableProps.Length)];
        }

        /// <summary>
        ///     Gets a prop based on its volume and heatmap position
        /// </summary>
        /// <param name="heatmapPosition">Input heatmap position</param>
        /// <returns>Prop</returns>
        private Prop GetPropByVolume(MapValue heatmapPosition)
        {
            // Error check
            if (spawnableProps.Length <= 0)
                return null;

            // Get heatmap value percentage. This is our location in range from threshold to max
            float hm_percent = (heatmapPosition.value - propThreshold) / (1 - propThreshold);
            // Get our volume range
            Vector2 volumeRange = new Vector2(float.MaxValue, float.MinValue);
            foreach (Prop prop in spawnableProps)
            {
                // Check minimum
                if (volumeRange.x > prop.volume)
                    volumeRange.x = prop.volume;
                // Check maximum
                if (volumeRange.y < prop.volume)
                    volumeRange.y = prop.volume;
            }

            // Get our target volume
            float targetVolume = Mathf.Lerp(volumeRange.x, volumeRange.y, hm_percent);
            targetVolume += UnityEngine.Random.Range(-0.05f * targetVolume, 0.05f * targetVolume);

            // Find the closest volume
            Prop minVolumeProp = spawnableProps[0];
            float minVolumeSeperation = Mathf.Abs(minVolumeProp.volume - targetVolume);

            for (int i = 0; i < spawnableProps.Length; i++)
            {
                float seperation = Mathf.Abs(spawnableProps[i].volume - targetVolume);
                if (minVolumeSeperation > seperation)
                {
                    minVolumeProp = spawnableProps[i];
                    minVolumeSeperation = seperation;
                }
            }

            return minVolumeProp;
        }
        #endregion

        #region Pruning
        /// <summary>
        ///     Finializes the bundle by getting rid of unwanted scripts
        /// </summary>
        public void Prune()
        {
            for (int i = 0; i < spawnedProps.Count; i++)
            {
                #if UNITY_EDITOR
                DestroyImmediate(spawnedProps[i]);
                #else
                Destroy(spawnedProps[i]);
                #endif
            }
            spawnedProps.Clear();

            
            #if UNITY_EDITOR
            DestroyImmediate(this);
            #else
            Destroy(this);
            #endif
        }
        #endregion
        #region Settings
        private Quaternion EvaluateRotationSettings()
        {
            switch (rotationSettings)
            {
                case RotationSettings.None:
                    return Quaternion.identity;

                case RotationSettings.X:
                    return Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.right);
                case RotationSettings.Y:
                    return Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up);
                case RotationSettings.Z:
                    return Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.forward);

                case RotationSettings.XY:
                    return Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), 0));
                case RotationSettings.XZ:
                    return Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), 0, UnityEngine.Random.Range(0, 360)));
                case RotationSettings.YZ:
                    return Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

                default: return UnityEngine.Random.rotation;
            }
        }
        #endregion
        #region Debug
        private void OnDrawGizmos()
        {
            // Draw map
            heatmap.Debug_DrawGizmos();
            // Draw bounds
            Gizmos.color = Color.blue;
            bounds.center = transform.position; // DONT LIKE THIS LINE... MOVE TO EDITOR
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            #if HFHANDY_DEVELOPMENT
            // Draw relevant verticies
            for(int i = 0; i < relevantVerticies.Length; i++)
            {
                Gizmos.color = HeatmapConstants.s_ValueGradient.Evaluate(0.5f + Vector3.Dot(relevantNormals[i], Vector3.up) / 2);

                // Draw point
                Gizmos.DrawSphere(relevantVerticies[i], 0.01f);
                // Draw normal
                Gizmos.DrawLine(relevantVerticies[i], relevantVerticies[i] + (relevantNormals[i] * 0.1f));
            }        
            #endif
        }
        #endregion
    }
}