using static Raylib_cs.Raylib;
using System.Numerics;

namespace SharpRay.Collision
{
    public class RectCollider : ICollider
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Raylib_cs.Rectangle Collider
        {
            get => new Raylib_cs.Rectangle
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }
        public bool ContainsPoint(Vector2 point) => CheckCollisionPointRec(point, Collider);
        public bool Overlaps(ICollider c) => CheckCollisionRecs(Collider, (c as RectCollider).Collider);
    }
}
