using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ImageAnalysis;

namespace Editor.ImageAnalyzer
{
    public class ImageAnalysisWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset = default;
        private Sprite InputSprite { get; set; }

        [MenuItem("Window/Services/Image Analysis")]
        public static void ShowWindow()
        {
            ImageAnalysisWindow wnd = GetWindow<ImageAnalysisWindow>();
            wnd.titleContent = new GUIContent("ImageAnalysisWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            
            ObjectField spriteField = new ObjectField("Input Sprite")
            {
                objectType = typeof(Sprite),
                allowSceneObjects = false
            };
            
            spriteField.RegisterValueChangedCallback(evt =>
            {
                InputSprite = evt.newValue as Sprite;
            });
            
            // Button
            Button analyzeTilemapButton = new Button(OnAnalyzeTilemapClick)
            {
                text = "Analyze Tilemap"
            };
            
            // Add to UI
            root.Add(spriteField);
            root.Add(analyzeTilemapButton);
        }

        private void OnAnalyzeTilemapClick()
        {
            ImageAnalysis.ImageAnalyzer imageAnalyzer = new ImageAnalysis.ImageAnalyzer(AssetDatabase.GetAssetPath(InputSprite), "Assets/Resources/Tiles/Pokemon/");
            imageAnalyzer.Run();
        }
    }
}
