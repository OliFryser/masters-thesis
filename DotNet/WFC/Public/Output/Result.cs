using System.Collections.Generic;
using Domain;

namespace WFC.Public.Output
{
    public class Result
    {
        public Result(IReadOnlyDictionary<Vector, TileType> map, Status status)
        {
            Map = map;
            Status = status;
        }

        public IReadOnlyDictionary<Vector, TileType> Map { get; }
        public Status Status { get; }
    }
}