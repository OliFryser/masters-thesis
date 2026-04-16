using System;

namespace MapElites.Models
{
    public abstract class BaseKey<T> : IEquatable<T> where T : BaseKey<T>
    {
        public abstract bool Equals(T? other);
        public abstract override int GetHashCode();
    }
}