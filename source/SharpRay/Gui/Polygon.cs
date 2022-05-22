using SharpRay.Core;

namespace SharpRay.Gui
{
    public sealed class Polygon : DragEditShape
    {
        public int Sides { get; }
        public float Radius { get; }

        // Points are ordered anti-clockwise and are drawn relative to Position, i.e. as if Position were 0,0.
        // If the points array is not closed, position will act as the 1st point and opens/closes the polygon.
        private Vector2[] Points { get; set; }
        private Vector2[] TextCoords { get; init; }

        private static Texture2D texture2D = new() { id = 1, }; // seems like textures need to be unloaded btw

        public Polygon(int sides, float radius)
        {
            Sides = sides;
            Radius = radius;
            Points = CreatePoints(Sides, Radius * Scale);
            TextCoords = Array.Empty<Vector2>();
        }

        public override bool ContainsPoint(Vector2 point)
        {
            for (var i = 0; i < Points.Length - 1; i++) 
            {
                if (CheckCollisionPointTriangle(point, Position, Points[i] + Position, Points[i + 1] + Position))
                    return true;

            }
            return false;
        }

        public override void Render()
        {
            base.Render();
            DrawTexturePoly(texture2D, Position, Points, TextCoords, Points.Length, ColorRender);
        }

        public override void OnMouseEvent(IMouseEvent me)
        {
            base.OnMouseEvent(me);

            if (me is MouseWheelUp || me is MouseWheelDown)
            {
                Points = CreatePoints(Sides, Radius * Scale);
            }
        }

        private static Vector2[] CreatePoints(int sides, float size)
        {
            var points = new Vector2[sides + 1];

            for (var i = 0; i <= sides; i++)
            {
                var t = i * MathF.Tau / sides;
                points[i] = new Vector2
                {
                    X = MathF.Sin(t) * size,
                    Y = MathF.Cos(t) * size
                };
            }
            return points;
        }
    }
}

