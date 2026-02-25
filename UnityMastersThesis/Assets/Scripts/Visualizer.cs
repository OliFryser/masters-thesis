using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private TextAsset _layoutFile;
    [SerializeField] private TextAsset _adjacencyFile;
    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private Tilemap _tilemap;
    
    public void DisplayMap()
    {
        string[] rows = _layoutFile.text.Split('\n', '\r').Where(r => !string.IsNullOrEmpty(r)).ToArray();
        Debug.Log(_tiles.Length);
        
        _tilemap.ClearAllTiles();
        
        for (int y = 0; y < rows.Length; y++)
        {
            string row = rows[y];
            string[] columns = row.Split(',');

            for (int x = 0; x < columns.Length; x++)
            {
                string tileId = columns[x];

                if (tileId == string.Empty)
                {
                    Debug.LogWarning($"Empty sprite ID at {x},{y}");
                    continue;
                }

                try
                {
                    TileBase tileBase = _tiles.First(tileBase => tileBase.name == tileId);
                    _tilemap.SetTile(new Vector3Int(y, -x, 0), tileBase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                
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
