using System;
using Models;

namespace WFC.Extensions
{
    public static class DirectionExtensions
    {
        public static Direction Reverse(this Direction direction) =>
            direction switch
            {
                Direction.North => Direction.South,
                Direction.West => Direction.East,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
    }
}