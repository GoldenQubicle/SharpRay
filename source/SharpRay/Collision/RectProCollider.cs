using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading;
using static Raylib_cs.Raylib;

namespace SharpRay.Collision
{
    public class RectProCollider : ICollider
    {
        public Vector2 Center { get; private set; }
        public Vector2 Size { get; }
        public float Rotation { get; private set; }
        private Vector2[] Points = new Vector2[4];
        public RectProCollider(Vector2 center, Vector2 size, float rotation)
        {
            Center = center;
            Size = size;
            Rotation = rotation;

            Points[0] = new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2);
            Points[1] = new Vector2(Center.X + Size.X / 2, Center.Y - Size.Y / 2);
            Points[2] = new Vector2(Center.X + Size.X / 2, Center.Y + Size.Y / 2);
            Points[3] = new Vector2(Center.X - Size.X / 2, Center.Y + Size.Y / 2);
        }



        public bool ContainsPoint(Vector2 point) { return false; }
        public bool Overlaps(ICollider collider)
        {
            /* kinda works as a first naive implementation the obvious issue here being;
             * it only detects whether or not the other collider has passed any of the lines,  
             * i.e. once the other collider is inside the rect it is no longer being detected as overlapping
             * while it actually is overlapping, obviously. And this WILL happen on occasion!
             * may just want to change this to point in triangle .. and keep the resolution along the circumference the same!
            */

            if (collider is CircleCollider cc)
            {
                foreach (var (start, end) in GetLines())
                {
                    for (var lerp = 0f; lerp < 1; lerp += .01f)
                    {
                        if (CheckCollisionPointCircle(Vector2.Lerp(start, end, lerp), cc.Center, cc.Radius))
                        {
                            return true;
                        }
                    }
                }
            }

            var rv = new Vector2();
            if (collider is RectProCollider rpc)
            {
                foreach (var line in GetLines())
                {
                    foreach (var otherLine in rpc.GetLines())
                    {
                        if (CheckCollisionLines(line.start, line.end, otherLine.start, otherLine.end, ref rv))
                        {
                            Console.WriteLine($"rect pro collieder collision {rv}");
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public void Render()
        {
            DrawLineV(Points[0], Points[1], Color.BLUE);
            DrawLineV(Points[1], Points[2], Color.BLUE);

            //DrawLineV(Points[2], Points[0], Color.BLUE);

            DrawLineV(Points[2], Points[3], Color.BLUE);
            DrawLineV(Points[3], Points[0], Color.BLUE);
        }

        public IEnumerable<(Vector2 start, Vector2 end)> GetLines()
        {
            for (var i = 0; i < Points.Length; i++)
                yield return i == 3 ? (Points[i], Points[0]) : (Points[i], Points[i + 1]);
        }
    }
}