using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MapElites.Extensions;
using Pokémon;
using SFB;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private ArchivePlaymodeExplorer _archivePlaymodeExplorer;

    private List<Key> _keys;
    
    private SliderInt _flowerSlider;
    private SliderInt _doorsSlider;
    private SliderInt _tileTypesSlider;
    private Button _runButton;
    private Button _pickArchiveButton;
    private RadioButtonGroup _tilemapRadioButtonGroup;
    private Label _customArchive;
    private string _customArchiveFilePath;

    public void OnEnable()
    {
        _runButton = _uiDocument.rootVisualElement.Q<Button>("RunButton");
        _pickArchiveButton = _uiDocument.rootVisualElement.Q<Button>("PickArchiveButton");
        _tilemapRadioButtonGroup = _uiDocument.rootVisualElement.Q<RadioButtonGroup>("TilemapButtonGroup");
        _customArchive = _uiDocument.rootVisualElement.Q<Label>("CustomArchive");
        
        _tilemapRadioButtonGroup.RegisterValueChangedCallback(_ => UpdateLoadStatus());
        _tilemapRadioButtonGroup.value = 2;
        
        _runButton.clicked += Run;
        _pickArchiveButton.clicked += PickArchive;
        
        UpdateLoadStatus();
    }

    public void OnDisable()
    {
        _tilemapRadioButtonGroup.UnregisterValueChangedCallback(_ => UpdateLoadStatus());
        _runButton.clicked -= Run;
        _pickArchiveButton.clicked -= PickArchive;
    }
    
    public void Initialize(IEnumerable<Key> keys)
    {
        _flowerSlider = _uiDocument.rootVisualElement.Q<SliderInt>("FlowersSlider");
        _tileTypesSlider = _uiDocument.rootVisualElement.Q<SliderInt>("TileTypesSlider");
        
        _keys = keys.ToList();

        int minFlowers = _keys.Select(k => k.FlowerBucket).Min();
        int maxFlowers = _keys.Select(k => k.FlowerBucket).Max();

        int minTileTypes = _keys.Select(k => k.VariationBucket).Min();
        int maxTileTypes = _keys.Select(k => k.VariationBucket).Max();
        
       SetSlider(_flowerSlider, minFlowers, maxFlowers);
       SetSlider(_tileTypesSlider, minTileTypes, maxTileTypes);
    }

    private void PickArchive()
    {
        var extensions = new [] {
            new ExtensionFilter("JSON Files", "json"),
            new ExtensionFilter("All Files", "*" ),
        };

        try
        {
            StandaloneFileBrowser.OpenFilePanelAsync(
                "Open Archive File", 
                "", 
                extensions, 
                false, 
                paths =>
                {
                    if (paths.Length != 1)
                        return;
                    _customArchiveFilePath = paths.Single();
                    UpdateLoadStatus();
                });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void UpdateLoadStatus()
    {
        if (_tilemapRadioButtonGroup.value == 3)
        {
            if (string.IsNullOrEmpty(_customArchiveFilePath))
            {
                _customArchive.text = "No custom archive file loaded.";
                return;
            }
            _archivePlaymodeExplorer.LoadArchive(_customArchiveFilePath);
            _customArchive.text = "Archive Loaded.";
            return;
        }
        
        string folderName = _tilemapRadioButtonGroup.value switch
        {
            0 => "Letters",
            1 => "Arrows",
            2 => "Pokemon",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        string folderPath = Path.Combine(Application.streamingAssetsPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            throw new ArgumentException($"Folder {folderPath} does not exist.");
        }

        string[] files =  Directory.GetFiles(folderPath);
        
        if (files.Length == 0)
        {
            throw new ArgumentException($"Folder {folderPath} does not contain any files.");
        }

        string archiveFile = Directory.GetFiles(folderPath).FirstOrDefault();
        
        _archivePlaymodeExplorer.LoadArchive(archiveFile);
    }

    private void Run()
    {
        Key key = GetKey();
        
        ArchivePlaymodeExplorer.TilemapDomain tilemapDomain = _tilemapRadioButtonGroup.value switch
        {
            0 => ArchivePlaymodeExplorer.TilemapDomain.Letters,
            1 => ArchivePlaymodeExplorer.TilemapDomain.Arrows,
            2 => ArchivePlaymodeExplorer.TilemapDomain.Pokemon,
            3 => ArchivePlaymodeExplorer.TilemapDomain.ReadFromArchive,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _archivePlaymodeExplorer.BrowseConstrainedArchive(key, tilemapDomain);
    }

    private Key GetKey()
    {
        return _keys.MinBy(k =>
            Mathf.Abs(k.FlowerBucket - _flowerSlider.value) + 
            Mathf.Abs(k.VariationBucket - _tileTypesSlider.value));
    }

    private static void SetSlider(SliderInt slider, int min, int max)
    {
        slider.lowValue = min;
        slider.highValue = max;
        slider.value = (min + max) / 2;
    }
}