using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using Tile = Domain.Models.Tile;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private TextAsset _layoutFile;
    [SerializeField] private TextAsset _adjacencyFile;
    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private WfcArgs _wfcArgs;
    
    [Button("Display Map from layout file")]
    public void DisplayMapFromJson()
    {
        List<Domain.Models.Tile> tileLayout =
            JsonConvert.DeserializeObject<List<Domain.Models.Tile>>(_layoutFile.text);
        
        _tilemap.ClearAllTiles();
        
        DisplayTiles(tileLayout);
    }

    private void DisplayTiles(List<Tile> tileLayout)
    {
        foreach (Domain.Models.Tile tile in tileLayout)
        {    
            try
            {
                TileBase tileBase = _tiles.First(tileBase => tileBase.name == tile.Name);
                _tilemap.SetTile(new Vector3Int(tile.Position.X, -tile.Position.Y, 0), tileBase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    [Button("Generate and display WFC map")]
    public void GenerateAndDisplayWfcMap()
    {
        _tilemap.ClearAllTiles();
        var tiles = _wfcArgs.RunWfc();
        DisplayTiles(tiles);
    }
    
    public void ReadAdjacencyData()
    {
        Dictionary<string, Adjacency> adjacencyData =
            JsonUtility.FromJson<Dictionary<string, Adjacency>>(_adjacencyFile.text);
        Debug.Log(adjacencyData.Count);
    }
    
    [Serializable]
    public class Adjacency
    {
        public Dictionary<string, int> UpNeighbors { get; set; } = new();
        public Dictionary<string, int> DownNeighbors { get; set; } = new();
        public Dictionary<string, int> LeftNeighbors { get; set; } = new();
        public Dictionary<string, int> RightNeighbors { get; set; } = new();
    }
}
