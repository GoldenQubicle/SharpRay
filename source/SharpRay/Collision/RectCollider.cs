using static Raylib_cs.Raylib;
using System.Numerics;
using Raylib_cs;
using System;

namespace SharpRay.Collision
{
    public class RectCollider : ICollider
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
        public bool ContainsPoint(Vector2 point) => CheckCollisionPointRec(point, Collider);
        
        public bool Overlaps(ICollider collider) => collider switch
        {
            RectCollider rc => CheckCollisionRecs(Collider, rc.Collider),
            CircleCollider cc => CheckCollisionCircleRec(cc.Center, cc.Radius, Collider),
            _ => throw new NotImplementedException($"Rect collider does not provide overlap check for {collider.GetType().Name}")
        };

        public void Render()
        {
            DrawRectangleLinesEx(Collider, 2, Color.BLUE);
        }

    }
}
