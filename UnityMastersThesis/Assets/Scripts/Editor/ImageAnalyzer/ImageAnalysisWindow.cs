using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Editor.ImageAnalyzer
{
    public class ImageAnalysisWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        private Sprite InputSprite { get; set; }
        private Button ConvertTilesButton { get; set; }
        
        private const string BaseDirectory = "Assets/Resources/Tiles/Pokemon/";
        
        private string ParentDirectory { get; set; }
        
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
                ParentDirectory = BaseDirectory + evt.newValue.name;
            });
            
            Button analyzeTilemapButton = new Button(AnalyzeTilemap)
            {
                text = "Analyze Tilemap"
            };

            ConvertTilesButton = new Button(ConvertTiles)
            {
                text = "Convert Tiles", 
            };
            ConvertTilesButton.SetEnabled(HasTilemap());
            
            // Add to UI
            root.Add(spriteField);
            root.Add(analyzeTilemapButton);
            root.Add(ConvertTilesButton);
        }

        private void AnalyzeTilemap()
        {
            ImageAnalysis.ImageAnalyzer imageAnalyzer = 
                new ImageAnalysis.ImageAnalyzer(AssetDatabase.GetAssetPath(InputSprite), ParentDirectory);
            imageAnalyzer.Run();
            AssetDatabase.Refresh();
            ConvertTilesButton.SetEnabled(HasTilemap());
        }

        private void ConvertTiles()
        {
            var guids = GetTiles();
            string tileFolder = ParentDirectory + "/Tiles";
            if (!AssetDatabase.IsValidFolder(tileFolder))
                AssetDatabase.CreateFolder(ParentDirectory, "Tiles");
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 16;
                    importer.filterMode = FilterMode.Point;
                    importer.mipmapEnabled = false;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaIsTransparency = true;

                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
                
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (sprite != null)
                {
                    Tile tile = CreateInstance<Tile>();
                    tile.sprite = sprite;

                    string tilePath = $"{tileFolder}/{sprite.name}.asset";
                    AssetDatabase.CreateAsset(tile, tilePath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string[] GetTiles()
            => AssetDatabase.FindAssets("t:Texture2D", new[] { ParentDirectory + "/TileSprites/" });


        private bool HasTilemap()
            => InputSprite != null;
    }
}
