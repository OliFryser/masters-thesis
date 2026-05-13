using System.Collections.Generic;
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
    private bool _archiveLoaded;

    public void OnEnable()
    {
        _runButton = _uiDocument.rootVisualElement.Q<Button>("RunButton");
        _pickArchiveButton = _uiDocument.rootVisualElement.Q<Button>("PickArchiveButton");
        _tilemapRadioButtonGroup = _uiDocument.rootVisualElement.Q<RadioButtonGroup>("TilemapButtonGroup");
        _runButton.SetEnabled(false);
        
        _tilemapRadioButtonGroup.RegisterValueChangedCallback(_ => UpdateLoadStatus());
        
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
        StandaloneFileBrowser.OpenFilePanelAsync(
            "Open Archive File", 
            "", 
            extensions, 
            false, 
            paths =>
            {
                if (paths.Length != 1)
                    return;
                _archivePlaymodeExplorer.LoadArchive(paths.Single());
                _archiveLoaded = true;
                _pickArchiveButton.style.backgroundColor = new Color(.5f, 1f, 0.45f);
                UpdateLoadStatus();
            });
    }

    private void UpdateLoadStatus()
    {
        _runButton.SetEnabled(_archiveLoaded && _tilemapRadioButtonGroup.value != -1);
    }

    private void Run()
    {
        Key key = GetKey();
        _archivePlaymodeExplorer.BrowseConstrainedArchive(key, _tilemapRadioButtonGroup.value);
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