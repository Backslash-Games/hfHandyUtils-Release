using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace HFHandyUtils.Data.Props
{
    [CustomEditor(typeof(Prop)), CanEditMultipleObjects]
    public class PropEditor : Editor
    {
        /// <summary>
        ///     Help box that notifies the user about a desynced boundary
        /// </summary>
        HelpBox _errorBounds;

        string _storedMemoryAllocation = "Please update manually";

        #region Editor Overrides
        /// <summary>
        ///     Creates a custom UI for the inspector
        /// </summary>
        /// <returns>Custom UI - Visual Element</returns>
        public override VisualElement CreateInspectorGUI()
        {
            // Create the root to our custom inspector UI
            VisualElement _root = new VisualElement();

            // Error check
            if (!(serializedObject.targetObject is MonoBehaviour mb))
            {
                Label error_NonMono = new Label("Prop is not listed as a MonoBehaviour, please fix");
                _root.Add(error_NonMono);
                return _root;
            }

            // Track scene components
            Prop prop = (Prop)target;

            #if HFHANDY_DEVELOPMENT
            // Bind development information to root
            _root.Add(CreateDevelopmentInformation(prop));
            #endif



            // Info readout
            // Drop down field
            Foldout infoFoldout = new Foldout() { name = "infoFoldout", text = "<b>Info</b>" };
            infoFoldout.style.paddingBottom = 16;
            // -> Volume
            Label volumeLabel = new Label($"<b>Volume</b>: {prop.volume}");
            infoFoldout.Add(volumeLabel);
            // Finalize field
            _root.Add(infoFoldout);



            // Bound update
            // -> Flag to Allow bound update on transform modified
            Toggle transformUpdateField = new Toggle("Allow Bound Update on Transform Modification") { bindingPath = "transformMeshUpdate" };
            _root.Add(transformUpdateField);
            // ->  Update bounds
            _root.Add(CreateBoundUpdateButton(prop));



            // Mesh modifications
            // -> Mesh
            ObjectField meshField = new ObjectField("Mesh") { bindingPath = "mesh" };
            meshField.objectType = typeof(Mesh);
            _root.Add(meshField);
            // -> Material
            ObjectField materialField = new ObjectField("Material") { bindingPath = "material" };
            materialField.style.paddingBottom = 16;
            materialField.objectType = typeof(Material);
            _root.Add(materialField);



            // Action buttons
            // -> Snap to nearest prop
            Button snapToPropButton = new Button() { name = "snapToPropButton", text = "Snap to nearest prop" };
            snapToPropButton.clicked += prop.ConnectToNearest;
            _root.Add(snapToPropButton);




            // Handle help boxes
            // -> Error - bounds
            _errorBounds = new HelpBox("Please update mesh bounds manually, since mesh properties have changed.", HelpBoxMessageType.Error);
            _errorBounds.Add(CreateBoundUpdateButton(prop));
            _root.Add(_errorBounds);
            _errorBounds.style.display = DisplayStyle.None;



            // Track events
            // -> Mesh Changed
            _root.TrackPropertyValue(serializedObject.FindProperty("mesh"), (_) => { prop.TickMesh("Mesh Changed"); NotifyBoundsUpdate(false); });
            // -> Material Changed
            _root.TrackPropertyValue(serializedObject.FindProperty("material"), (_) => { prop.TickMesh("Material Changed"); NotifyBoundsUpdate(false); });
            // -> Transform Changed
            VisualElement transformElement = new VisualElement();
            transformElement.TrackSerializedObjectValue(new SerializedObject(prop.transform), (_) => 
            {
                if (prop.TickMesh_Transform("Transform Changed"))
                    NotifyBoundsUpdate(false);
                else
                    NotifyBoundsUpdate(true); 
            });
            _root.Add(transformElement);
            // -> Debug readout
            _root.TrackSerializedObjectValue(serializedObject, (_) => { volumeLabel.text = $"<b>Volume</b>: {prop.volume}"; });



            // Finalize
            return _root;
        }
        #endregion
        #region Development - Visual Elements
        /// <summary>
        ///     Organizes and creates a visual element for Development Information
        /// </summary>
        /// <param name="prop">Root Object</param>
        /// <returns>Visual Element</returns>
        private VisualElement CreateDevelopmentInformation(Prop prop)
        {
            // DEVELOPMENT - Element definition
            VisualElement developmentInformation = new VisualElement();

            // DEVELOPMENT - Header
            Label dev_Header = new Label("<b><color=#7A7A7A>START :: Development Properties</color></b>");
            developmentInformation.Add(dev_Header);

            // DEVELOPMENT - Create labels
            // -> Bound output
            Label dev_Bounds = new Label($"<b>Bounds</b>: {prop.boundingBox.ToString()}\n") { };
            developmentInformation.Add(dev_Bounds);
            // -> Check memory usage
            Button dev_UpdateMemoryAlloc = new Button() { name = "dev_UpdateMemoryAlloc", text = "Update Memory Allocation Information" };
            dev_UpdateMemoryAlloc.clicked += () => UpdateMemoryAllocationNotice(prop);
            developmentInformation.Add(dev_UpdateMemoryAlloc);
            // -> Current memory output
            Label dev_MemoryAlloc = new Label($"<b>Memory Allocation</b>: {_storedMemoryAllocation}") { };
            developmentInformation.Add(dev_MemoryAlloc);

            // DEVELOPMENT - Footer
            Label dev_Footer = new Label("<b><color=#7A7A7A>END :: Development Properties</color></b>\n\n");
            developmentInformation.Add(dev_Footer);

            // Track elements
            // -> Bounds
            developmentInformation.TrackSerializedObjectValue(serializedObject, (_) => 
            { dev_Bounds.text = $"<b>Bounds</b>: {prop.boundingBox.ToString()}\n"; });
            // -> Memory allocation
            developmentInformation.schedule.Execute(() => { dev_MemoryAlloc.text = $"<b>Memory Allocation</b>: {_storedMemoryAllocation}"; }).Every(100);

            return developmentInformation;
        }
        #endregion
        #region Quick Creation - Visual Elements
        /// <summary>
        ///     Creates a button that is used to update prop bounds
        /// </summary>
        /// <returns>New Button</returns>
        private Button CreateBoundUpdateButton(Prop prop)
        {
            Button updateBounds = new Button() { name = "updateMesh", text = "Update Bounds" };
            updateBounds.clicked += () => prop.TickMesh("Editor Update Bounds", true);
            updateBounds.clicked += () => NotifyBoundsUpdate(false);
            updateBounds.clicked += () => serializedObject.ApplyModifiedProperties();
            return updateBounds;
        }
        #endregion

        #region Bound Updating
        /// <summary>
        ///     Sets a flag that lets us know if we need to update bounds manually.
        ///     Could contain more logic
        /// </summary>
        /// <param name="state"></param>
        private void NotifyBoundsUpdate(bool state)
        {
            _errorBounds.style.display = state ? DisplayStyle.Flex : DisplayStyle.None; 
        }
        #endregion
        #region Memory Diagnosis
        /// <summary>
        ///     Updates the stored memory allocation
        /// </summary>
        /// <param name="prop">Object to check</param>
        private void UpdateMemoryAllocationNotice(Prop prop)
        {
            _storedMemoryAllocation = System.GC.GetTotalMemory(prop).ToString();
        }
        #endregion
    }
}