﻿namespace SharpRay.Collision
{
    public class RectProCollider : Collider
    {
        public Vector2 Center { get; set; }
        public Vector2 Size { get; private set; }

        private readonly Vector2[] points;

        public RectProCollider(Vector2 center, Vector2 size)
        {
            Center = center;
            Size = size;

            points = new Vector2[] 
            {
                new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2),
                new Vector2(Center.X + Size.X / 2, Center.Y - Size.Y / 2),
                new Vector2(Center.X + Size.X / 2, Center.Y + Size.Y / 2),
                new Vector2(Center.X - Size.X / 2, Center.Y + Size.Y / 2),
            };
        }
               
        public override void Render()
        {
            DrawCircleV(Center, 5, Orange);

            DrawLineV(points[0], points[1], Color);
            DrawLineV(points[1], points[2], Color);

            DrawLineV(points[2], points[0], Color);//diagonal to remind ourselves collision checks happen with 2 triangles

            DrawLineV(points[2], points[3], Color);
            DrawLineV(points[3], points[0], Color);
        }

        public IEnumerable<(Vector2 p1, Vector2 p2, Vector2 p3)> GetTriangles()
        {
            yield return (points[0], points[1], points[2]);
            yield return (points[2], points[3], points[0]);
        }

        public void Update(Matrix3x2 matrix)
        {
            Center = Vector2.Transform(Center, matrix);
            for (var i = 0; i < points.Length; i++)
                points[i] = Vector2.Transform(points[i], matrix);
        }

        public IEnumerable<(Vector2 start, Vector2 end)> GetLines()
        {
            for (var i = 0; i < points.Length; i++)
                yield return i == 3 ? (points[i], points[0]) : (points[i], points[i + 1]);
        }
    }
}