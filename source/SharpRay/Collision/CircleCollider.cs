using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay.Collision
{
    public class CircleCollider : Collider
    {
        public Vector2 Center { get; set; }
        public float Radius { get; init; }
        public int HitPoints { get; init; } = 4; //number of point on circumference used in collision detection with rect pro collider. 

        public IEnumerable<Vector2> GetHitPoints()
        {
            var theta = MathF.Tau / HitPoints;

            for (var i = 0; i < HitPoints; i++)
            {
                yield return Center + new Vector2(MathF.Cos(i * theta) * Radius, MathF.Sin(i * theta) * Radius);
            }
        }

        public override void Render()
        {
            foreach (var p in GetHitPoints())
                DrawCircleV(p, 2, Raylib_cs.Color.RED);

            DrawCircleLines((int)Center.X, (int)Center.Y, Radius, Color);
        }
    }
}
