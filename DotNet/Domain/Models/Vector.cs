using System;

namespace Domain.Models
{
    public struct Vector : IEquatable<Vector>
    {
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static bool operator ==(Vector left, Vector right) => left.Equals(right);
        public static bool operator !=(Vector left, Vector right) => !left.Equals(right);
        public bool Equals(Vector other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object? obj) => obj is Vector other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}