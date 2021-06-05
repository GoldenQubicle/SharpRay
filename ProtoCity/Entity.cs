using Raylib_cs;
using System;
using System.Numerics;

namespace ProtoCity
{
    public abstract class Entity
    {
        private int _id;
        public int Id
        {
            get => _id;
            init => _id = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(Id), $"Entity Id cannot be negative") ;
        }
    }

    public class Circle : Entity
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
    }

    public class Rectangle : Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }
    }
}
