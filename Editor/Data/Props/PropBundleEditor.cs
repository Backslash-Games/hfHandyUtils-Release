using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HFHandyUtils.Data.Props
{
    [CustomEditor(typeof(PropBundle)), CanEditMultipleObjects]
    public class PropBundleEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Root container
            VisualElement _root = new VisualElement();

            // Track scene components
            PropBundle propBundle = (PropBundle)target;

            // Create Variable fields
            // -> Spawnable props
            PropertyField spawnablePropProperty = new PropertyField(serializedObject.FindProperty("spawnableProps"));
            _root.Add(spawnablePropProperty);
            // -> Spawn Count
            PropertyField spawnCount = new PropertyField(serializedObject.FindProperty("spawnCount"));
            _root.Add(spawnCount);
            // -> Prop Threshold
            PropertyField propThresholdProperty = new PropertyField(serializedObject.FindProperty("propThreshold"));
            _root.Add(propThresholdProperty);
            // -> Spawn Settings
            PropertyField spawnSettingsProperty = new PropertyField(serializedObject.FindProperty("spawnSettings"));
            _root.Add(spawnSettingsProperty);
            // -> Rotation Settings
            PropertyField rotationSettingsProperty = new PropertyField(serializedObject.FindProperty("rotationSettings"));
            _root.Add(rotationSettingsProperty);
            Space(_root);

            // -> Heatmap Mask
            PropertyField heatmapMaskProperty = new PropertyField(serializedObject.FindProperty("heatmapMeshMask"));
            _root.Add(heatmapMaskProperty);
            // -> Heatmap
            PropertyField heatmapProperty = new PropertyField(serializedObject.FindProperty("heatmap"));
            _root.Add(heatmapProperty);

            // -> Bounds
            PropertyField boundsProperty = new PropertyField(serializedObject.FindProperty("bounds"));
            _root.Add(boundsProperty);
            Space(_root);


            // Create a button to update bundle
            Button bakeBundle = new Button() { name = "bakeBundle", text = "Bake Bundle"};
            bakeBundle.clicked += propBundle.BuildBundle;
            _root.Add(bakeBundle);
            // Create a button to clear bundle
            Button clearBundle = new Button() { name = "clearBundle", text = "Clear Bundle" };
            clearBundle.clicked += propBundle.Reset;
            _root.Add(clearBundle);
            Space(_root);

            // Create a button to prune bundle
            Button pruneBundle = new Button() { name = "pruneBundle", text = "Finalize Bundle" };
            pruneBundle.clicked += propBundle.Prune;
            _root.Add(pruneBundle);
            Space(_root);

            return _root;
        }

        private void Space(VisualElement root)
        {
            Label space = new Label(" ");
            root.Add(space);
        }
    }
}