using System;
using System.Collections.Generic;
using System.Linq;
using MapElites.Extensions;
using MapElites.Models;
using Pokémon;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private ArchivePlaymodeExplorer _archivePlaymodeExplorer;

    private List<Key> _keys;
    
    private SliderInt _flowerSlider;
    private SliderInt _doorsSlider;
    private SliderInt _tileTypesSlider;
    
    public void Initialize(IEnumerable<Key> keys)
    {
        _flowerSlider = _uiDocument.rootVisualElement.Q<SliderInt>("FlowersSlider");
        _doorsSlider = _uiDocument.rootVisualElement.Q<SliderInt>("DoorsSlider");
        _tileTypesSlider = _uiDocument.rootVisualElement.Q<SliderInt>("TileTypesSlider");
        _uiDocument.rootVisualElement.Q<Button>().clicked += Run;
        
        _keys = keys.ToList();

        int minFlowers = _keys.Select(k => k.FlowerBucket).Min();
        int maxFlowers = _keys.Select(k => k.FlowerBucket).Max();

        int minDoors = _keys.Select(k => k.DoorBucket).Min();
        int maxDoors = _keys.Select(k => k.DoorBucket).Max();

        int minTileTypes = _keys.Select(k => k.TileTypesUsedBucket).Min();
        int maxTileTypes = _keys.Select(k => k.TileTypesUsedBucket).Max();
        
       SetSlider(_flowerSlider, minFlowers, maxFlowers);
       SetSlider(_doorsSlider, minDoors, maxDoors);
       SetSlider(_tileTypesSlider, minTileTypes, maxTileTypes);

       Run();
    }

    private void Run()
    {
        Key key = GetKey();
        _archivePlaymodeExplorer.BrowseConstrainedArchive(key);
    }

    private Key GetKey()
    {
        return _keys.MinBy(k =>
            Mathf.Abs(k.FlowerBucket - _flowerSlider.value) + 
            Mathf.Abs(k.DoorBucket - _doorsSlider.value) + 
            Mathf.Abs(k.TileTypesUsedBucket - _tileTypesSlider.value));
    }

    private static void SetSlider(SliderInt slider, int min, int max)
    {
        slider.lowValue = min;
        slider.highValue = max;
        slider.value = (min + max) / 2;
    }
}