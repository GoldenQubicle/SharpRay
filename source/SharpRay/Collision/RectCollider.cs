using static Raylib_cs.Raylib;
using System.Numerics;
using Raylib_cs;

namespace SharpRay.Collision
{
    /// <summary>
    /// Basic rectangle collider. Cannot be rotated. 
    /// </summary>
    public class RectCollider : Collider
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Rectangle Collider
        {
            get => new Rectangle
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }
        public override void Render() => DrawRectangleLinesEx(Collider, 2, Color);
        
    }
}
