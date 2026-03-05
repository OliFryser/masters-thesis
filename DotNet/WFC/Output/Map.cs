using System.Collections.Generic;
using Domain.Models;
using WFC.Args;

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