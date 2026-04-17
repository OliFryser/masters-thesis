using System.Collections.Generic;
using System.Linq;
using TilemapAnalysis;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Tile = UnityEngine.Tilemaps.Tile;

namespace Editor.ImageAnalyzer
{
    public class ImageAnalysisWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        private Sprite InputSprite { get; set; }
        private Button CreateConfigFromTilemapButton { get; set; }
        
        private const string BaseDirectory = "Assets/Resources/Tiles/Pokemon/";
        private const string ConfigDirectory = "Assets/Scriptable Objects/WfcConfigs/";
        
        private string ParentDirectory { get; set; }
        
        [MenuItem("Window/Services/Image Analysis")]
        public static void ShowWindow()
        {
            ImageAnalysisWindow window = GetWindow<ImageAnalysisWindow>();
            window.titleContent = new GUIContent("Image Analysis Service");
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
            
            CreateConfigFromTilemapButton = new Button(CreateConfigFromTilemap)
            {
                text = "Create Wfc Configuration from Tilemap", 
            };
            
            // Add to UI
            root.Add(spriteField);
            root.Add(CreateConfigFromTilemapButton);
        }


        private void CreateConfigFromTilemap()
        {
            using var tilemapAnalyzer = new TilemapAnalyzer(AssetDatabase.GetAssetPath(InputSprite));

            List<TileBase> tiles = HasTiles() ? GetTiles().ToList() : ConvertTiles(tilemapAnalyzer);
            List<Domain.Models.AdjacencyRule> rules = tilemapAnalyzer.GetAdjacencyRules();
            List<AdjacencyRule> convertedRules = new List<AdjacencyRule>();
            foreach (var rule in rules)
            {
                var fromTile = tiles.First(t => t.name == rule.From.Id);
                var toTile = tiles.First(t => t.name == rule.To.Id);
                var convertedRule = new AdjacencyRule(fromTile, toTile, rule.Direction);
                convertedRules.Add(convertedRule);
            }

            WfcConfig wfcConfig = CreateInstance<WfcConfig>();
            wfcConfig.name = $"{InputSprite.name}_WfcConfig";
            wfcConfig.Tiles = tiles.ToArray();
            wfcConfig.Rules = convertedRules.ToArray();
            wfcConfig.Weights = tilemapAnalyzer.Weights.Select(w => new SerializedTileWeight(w)).ToList();

            AssetDatabase.CreateAsset(wfcConfig, ConfigDirectory + wfcConfig.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private List<TileBase> ConvertTiles(TilemapAnalyzer tilemapAnalyzer)
        {
            string tileSpritesFolder = ParentDirectory + "/TileSprites";
            if (!AssetDatabase.IsValidFolder(tileSpritesFolder))
                AssetDatabase.CreateFolder(ParentDirectory, "TileSprites");
            
            tilemapAnalyzer.WriteTileSpritesToFolder(tileSpritesFolder);
            AssetDatabase.Refresh();
            
            var guids = GetTilesSprites();
            
            string tileFolder = ParentDirectory + "/Tiles";
            if (!AssetDatabase.IsValidFolder(tileFolder))
                AssetDatabase.CreateFolder(ParentDirectory, "Tiles");
            
            List<TileBase> convertedTiles = new List<TileBase>();
            guids.ToList().ForEach(guid =>
            {
                TileBase convertedTile = ConvertTile(guid, tileFolder);
                if (convertedTile != null)
                {
                    convertedTiles.Add(convertedTile);
                }
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return convertedTiles;
        }

        private static TileBase ConvertTile(string guid, string tileFolder)
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
                
                return tile;
            }

            return null;
        }

        private bool HasTiles()
            => GetTiles().Any();

        private TileBase[] GetTiles()
        {
            string[] guids = AssetDatabase.FindAssets("t:Tile", new[] { ParentDirectory + "/Tiles/" });
            return guids.Select(guid =>
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<TileBase>(assetPath);
                })
                .Where(tile => tile != null) // Safety check: filter out any failed loads
                .ToArray();
        }
        
        private string[] GetTilesSprites()
            => AssetDatabase.FindAssets("t:Texture2D", new[] { ParentDirectory + "/TileSprites/" });
    }
}
