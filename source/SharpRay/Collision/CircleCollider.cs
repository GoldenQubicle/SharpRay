using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay.Collision
{
    public class CircleCollider : ICollider
    {
        public Vector2 Center { get; init; }
        public float Radius { get; init; }

        public bool ContainsPoint(Vector2 point) => CheckCollisionPointCircle(point, Center, Radius);
        public bool Overlaps(ICollider collider) => collider switch
        {
            RectCollider rc => CheckCollisionCircleRec(Center, Radius, rc.Collider),
            CircleCollider cc => CheckCollisionCircles(Center, Radius, cc.Center, cc.Radius),
            _ => throw new NotImplementedException($"Circle collider does not provide overlap check for {collider.GetType().Name}")
        };

        public void Render() => DrawCircleLines((int)Center.X, (int)Center.Y, Radius, Color.BLUE);
    }
}
