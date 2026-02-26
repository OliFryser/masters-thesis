using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text.Json;
using Newtonsoft.Json;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private TextAsset _layoutFile;
    [SerializeField] private TextAsset _adjacencyFile;
    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private Tilemap _tilemap;
    
    public void DisplayMap()
    {
        List<ImageAnalysis.Models.Tile> tileLayout =
            JsonConvert.DeserializeObject<List<ImageAnalysis.Models.Tile>>(_layoutFile.text);
        
        _tilemap.ClearAllTiles();
        
        foreach (ImageAnalysis.Models.Tile tile in tileLayout)
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
