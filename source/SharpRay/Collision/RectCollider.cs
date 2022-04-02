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
        public Rectangle Rect
        {
            get => new()
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }
        public override void Render() => DrawRectangleLinesEx(Rect, 2, Color);
        
    }
}
