using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Extensions;
using MapElites.Models;
using Domain;
using SFB;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private ArchivePlaymodeExplorer _archivePlaymodeExplorer;

    [SerializeField] private Color _minFitnessColor;
    [SerializeField] private Color _maxFitnessColor;
    [SerializeField] private Color _minFeasibleColor;
    [SerializeField] private Color _maxFeasibleColor;

    private List<Key> _keys;
    private int _bucketsPerAxis;

    private RadioButtonGroup _behaviorButtonsGroup;
    private Button _runButton;
    private Label _value1;
    private Label _value2;
    private Label _value3;
    private Label _value4;

    private EnumField _archiveType;

    public void OnEnable()
    {
        _runButton = _uiDocument.rootVisualElement.Q<Button>("RunButton");
        _archiveType = _uiDocument.rootVisualElement.Q<EnumField>("ArchiveType");
        _archiveType.value = ArchiveType.Pokémon;
        _value1 = _uiDocument.rootVisualElement.Q<Label>("v1");
        _value2 = _uiDocument.rootVisualElement.Q<Label>("v2");
        _value3 = _uiDocument.rootVisualElement.Q<Label>("v3");
        _value4 = _uiDocument.rootVisualElement.Q<Label>("v4");
        _behaviorButtonsGroup = _uiDocument.rootVisualElement.Q<RadioButtonGroup>("BehaviorButtons");

        _behaviorButtonsGroup.RegisterValueChangedCallback(_ => UpdateValueLabels());

        _archiveType.RegisterValueChangedCallback(_ => UpdateLoadStatus());

        _runButton.clicked += Run;

        UpdateLoadStatus();
    }

    public void OnDisable()
    {
        _behaviorButtonsGroup.UnregisterValueChangedCallback(_ => UpdateValueLabels());
        _runButton.clicked -= Run;
    }

    public void Initialize(
        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive)
    {
        VisualElement buttonContainer = _behaviorButtonsGroup.Q<VisualElement>("choicesContentContainer");
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.flexWrap = Wrap.Wrap;
        buttonContainer.Clear();

        _keys = archive.GetKeys().ToList();
        _bucketsPerAxis = (int)Mathf.Sqrt(archive.BucketCapacity);

        float percentage = 100f / _bucketsPerAxis - 0.01f;

        for (int i = 0; i < _bucketsPerAxis * _bucketsPerAxis; i++)
        {
            RadioButton radioButton = new RadioButton
            {
                name = $"Bucket{i}",
            };
            radioButton.AddToClassList("archive-buttons");

            // Apply the layout to the RadioButton wrapper
            radioButton.style.width = Length.Percent(percentage);
            radioButton.style.aspectRatio = 1;

            radioButton.style.marginLeft = 0;
            radioButton.style.marginRight = 0;
            radioButton.style.marginTop = 0;
            radioButton.style.marginBottom = 0;

            Key key = GetKeyFromIndex(i);
            if (archive.TryGet(key, out var entry))
            {
                VisualElement inputBox = radioButton.Q(className: "unity-radio-button__input");
                if (inputBox != null)
                {
                    if (entry.IsFeasible)
                    {
                        // Color color = Color.Lerp(_minFitnessColor, _maxFitnessColor, entry.Fitness);
                        Color color = ViridisColor.GetColor(entry.Fitness);
                        inputBox.style.backgroundColor = color;
                    }
                    else
                    {
                        Color color = Color.Lerp(_minFeasibleColor, _maxFeasibleColor, entry.Fitness);
                        inputBox.style.backgroundColor = color;
                    }
                }
            }

            radioButton.SetEnabled(_keys.Contains(key));

            buttonContainer.Add(radioButton);
        }

        SelectValidKey();
    }

    private void SelectValidKey()
    {
        List<int> options = new();
        for (int i = 0; i < _bucketsPerAxis * _bucketsPerAxis; i++)
        {
            Key key = GetKeyFromIndex(i);

            if (_keys.Contains(key))
            {
                options.Add(i);
            }
        }

        _behaviorButtonsGroup.value = options.GetRandomElement();
    }

    private Key GetKeyFromIndex(int index)
    {
        int xBehavior = index % _bucketsPerAxis;
        // Flip the y-axis since 0,0 is bottom-left
        int yBehavior = (_bucketsPerAxis - 1) - (index / _bucketsPerAxis);

        return new Key(xBehavior, yBehavior);
    }

    private void UpdateValueLabels()
    {
        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive =
            _archivePlaymodeExplorer.ConstrainedArchive;
        Key key = GetKey();
        if (archive.TryGet(key, out ConstrainedEntry<Individual, Behavior> entry))
        {
            _value1.text = $"{Mathf.Round(entry.Behavior.SpecialTilePercentage * 100)}%";
            _value2.text = $"{Mathf.Round(entry.Behavior.Variation * 100)}%";
            _value3.text = $"{Mathf.Round(entry.Feasibility * 100)}%";
            _value4.text = $"{entry.Fitness:F2}";
        }
    }

    private void PickArchive()
    {
        var extensions = new[]
        {
            new ExtensionFilter("JSON Files", "json"),
            new ExtensionFilter("All Files", "*"),
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

                    var archivePath = paths.Single();

                    if (string.IsNullOrEmpty(archivePath))
                    {
                        return;
                    }

                    _archivePlaymodeExplorer.LoadArchiveFile(archivePath);

                    SelectValidKey();
                });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void UpdateLoadStatus()
    {
        if ((ArchiveType)_archiveType.value == ArchiveType.Custom)
        {
            PickArchive();
            return;
        }

        string folderName = (ArchiveType)_archiveType.value switch
        {
            ArchiveType.Letters => "Letters",
            ArchiveType.Arrows => "Arrows",
            ArchiveType.Pokémon => "Pokemon",
            _ => throw new ArgumentOutOfRangeException()
        };

        var textAssets = Resources.LoadAll<TextAsset>(folderName);

        var archiveFile = textAssets.Single();

        _archivePlaymodeExplorer.LoadArchive(archiveFile.text);

        SelectValidKey();
    }

    private void Run()
    {
        Key key = GetKey();

        ArchivePlaymodeExplorer.TilemapDomain tilemapDomain = _archiveType.value switch
        {
            ArchiveType.Letters => ArchivePlaymodeExplorer.TilemapDomain.Letters,
            ArchiveType.Arrows => ArchivePlaymodeExplorer.TilemapDomain.Arrows,
            ArchiveType.Pokémon => ArchivePlaymodeExplorer.TilemapDomain.Pokemon,
            ArchiveType.Custom => ArchivePlaymodeExplorer.TilemapDomain.ReadFromArchive,
            _ => throw new ArgumentOutOfRangeException()
        };

        _archivePlaymodeExplorer.BrowseConstrainedArchive(key, tilemapDomain);
    }

    private Key GetKey() => GetKeyFromIndex(_behaviorButtonsGroup.value);
}