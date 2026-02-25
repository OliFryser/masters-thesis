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
            
            
            ObjectField spriteField = new ObjectField("Tilemap Sprite")
            {
                objectType = typeof(Sprite),
                allowSceneObjects = false
            };
            
            
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
            ImageAnalysis.ImageAnalysis.Run();
        }
    }
}
