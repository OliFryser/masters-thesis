using System.Collections.Generic;
using Core.Models;

namespace WFC.Output
{
    public class Map
    {
        public Map(List<Tile> coordinateToTile)
        {
            Tiles = coordinateToTile;
        }
        
        public List<Tile> Tiles { get; }
    }
}