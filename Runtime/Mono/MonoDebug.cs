using UnityEngine;
using TMPro;
using HFHandyUtils.Graphical;

namespace HFHandyUtils.Mono
{
    /// <summary>
    ///     Scene Object that prints organized monobehaviour ToString information
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/MonoDebug-34ad086035d380548266e899cc3ea4b0">Documentation</a></br>
    /// </summary>
    [AddComponentMenu("HFHandyUtils/MonoBehaviour/MonoDebug")]
    public class MonoDebug : MonoBehaviour
    {
        /// <summary>
        ///     When enabled, on Awake this MonoDebug will set suppressDebug to true. This action prints to the console and declares source.
        /// </summary>
        [SerializeField] private bool suppressAllMonoDebug = false;
        /// <summary>
        ///     When enabled, it will suppress this MonoDebug
        /// </summary>
        [SerializeField] private bool suppressLocalMonoDebug = false;
        /// <summary>
        ///     Universal suppression flag
        /// </summary>
        public static bool suppressDebug = false;

        /// <summary>
        ///     Declares DebugStyle.World canvas offset
        /// </summary>
        [Space][SerializeField] private Vector3 offset;
        /// <summary>
        ///     Default DebugStyle.World canvas offset
        /// </summary>
        private static readonly Vector3 defaultOffset = new Vector3(0, 2.5f, 0);
        /// <summary>
        ///     Declares text color
        /// </summary>
        [SerializeField] private Color textColor = Color.white;

        /// <summary>
        ///     Declares the build style of the canvas
        /// </summary>
        [Space][SerializeField] private DebugStyle debugStyle = DebugStyle.World;

        /// <summary>
        ///     States that correlate with style in which debug information is displayed
        /// </summary>
        private enum DebugStyle { Canvas, World }
        /// <summary>
        ///     Declares text allignment
        /// </summary>
        [SerializeField] private TextAlignmentOptions canvas_allignment = TextAlignmentOptions.TopLeft;
        /// <summary>
        ///     Declares the source of debug information
        /// </summary>
        [SerializeField] private MonoBehaviour source = null;

        /// <summary>
        ///     Reference to the built canvas
        /// </summary>
        private Canvas canvas;
        /// <summary>
        ///     Scale of DebugStyle.World canvas
        /// </summary>
        private static readonly float canvasScale_world = 2.5f;

        /// <summary>
        ///     Reference to DebugStyle.World built billboarding
        /// </summary>
        private Billboard billboard = null;

        /// <summary>
        ///     Reference to the built text
        /// </summary>
        private TextMeshProUGUI text;
        /// <summary>
        ///     Padding on text
        /// </summary>
        private static readonly float textPadding_canvas = 5;

        #region Unity Methods
        /// <summary>
        ///     Default Awake Method. Checks suppression and sets up billboard
        /// </summary>
        private void Awake()
        {
            // Right out of the gates check our suppression
            // -> This needs to stay as the first call in awake
            CheckSuppression();
            // -> Set up the billboard component
            billboard = gameObject.AddComponent<Billboard>();
        }
        /// <summary>
        ///     Default Start Method. Initializes MonoDebug
        /// </summary>
        private void Start()
        {
            // Build the inital components
            Initialize();
        }

        /// <summary>
        ///     Default Update Method. Runs Tick
        /// </summary>
        private void Update()
        {
            Tick();
        }
        #endregion
        #region Suppression Handling
        /// <summary>
        ///     Checks if we need to supress MonoDebug
        /// </summary>
        private void CheckSuppression()
        {
            // Check for a suppression
            if (suppressAllMonoDebug && !suppressDebug)
            {
                suppressDebug = true;
                Debug.LogWarning($"{name} has suppressed all Mono Debug Scripts");
            }

            // Check for a local suppression
            if (source == null)
            {
                suppressLocalMonoDebug = true;
                Debug.LogWarning($"{name} has no defined source");
            }
        }
        /// <summary>
        ///     Checks to see if the script is suppressed
        /// </summary>
        /// <returns>True when suppressed</returns>
        private bool isSuppressed()
        {
            return suppressDebug || suppressLocalMonoDebug;
        }
        #endregion

        #region Text Drawing
        /// <summary>
        ///     Builds the initial objects for debugging
        /// </summary>
        private void Initialize()
        {
            // Check if we are suppressed
            if (isSuppressed())
                return;
            // Build objects
            BuildObjects();

            // Draw style
            if (debugStyle.Equals(DebugStyle.World))
                Draw_World();
            else
                Draw_Canvas();
        }

        /// <summary>
        ///     Sets up components needed for DebugStyle.World
        /// </summary>
        private void Draw_World()
        {
            // Apply Settings
            canvas.renderMode = RenderMode.WorldSpace;

            // Apply Sizing
            canvas.GetComponent<RectTransform>().sizeDelta = text.rectTransform.sizeDelta = canvasScale_world * (Vector3.one + Vector3.right);
            // -> Correct sizing for billboarding
            text.rectTransform.localScale = new Vector3(1, -1, 1);
            canvas.worldCamera = Camera.main;
            text.fontSize = 0.15f;
            text.alignment = TextAlignmentOptions.BottomLeft;

            // Setup billboard
            if (billboard != null) { billboard.Initialize(true, canvas.transform); }
        }
        /// <summary>
        ///     Sets up components needed for DebugStyle.Canvas
        /// </summary>
        private void Draw_Canvas()
        {
            // Apply Settings
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            // Apply Text Sizing
            text.rectTransform.sizeDelta = new Vector2(Screen.width - textPadding_canvas, Screen.height - textPadding_canvas);
            text.fontSize = 18;
            text.alignment = canvas_allignment;

            // Setup billboard
            if (billboard != null) { billboard.Initialize(false, canvas.transform); }
        }

        /// <summary>
        ///     Builds draw and text objects
        /// </summary>
        private void BuildObjects()
        {
            // Make sure our draw object is built
            BuildDrawObject();
            // Make sure our text object is built
            BuildTextObject();
        }
        /// <summary>
        ///     Builds the draw object and orients in the world properly
        /// </summary>
        private void BuildDrawObject()
        {
            // Check if the object is null
            if (canvas != null)
                return;

            // Create obect
            GameObject sObject = new GameObject($"MonoDebug_Canvas_{source.name}");
            sObject.transform.parent = source.transform;
            // Add component
            canvas = sObject.gameObject.AddComponent<Canvas>();
            // Orient
            canvas.transform.localPosition = defaultOffset + offset;
        }
        /// <summary>
        ///     Builds the text object and orients in the world properly
        /// </summary>
        private void BuildTextObject()
        {
            // Check if the object is null
            if (text != null)
                return;

            // Create obect
            GameObject sObject = new GameObject($"MonoDebug_Text_{source.name}");
            sObject.transform.parent = canvas.transform;
            // Add component
            text = sObject.gameObject.AddComponent<TextMeshProUGUI>();
            // Orient
            text.transform.localPosition = Vector3.zero;

            // Color
            text.color = textColor;
        }
        #endregion
        #region Updating
        /// <summary>
        ///     Runs all update logic
        /// </summary>
        private void Tick()
        {
            // Update the text
            UpdateText();
        }

        /// <summary>
        ///     Updates text from source
        /// </summary>
        private void UpdateText()
        {
            // Make sure text is set
            if (text == null)
                return;

            // Update the text
            text.text = $"MonoDebug - Object:{source.name}\n{source.ToString()}";
        }
        #endregion
    }
}