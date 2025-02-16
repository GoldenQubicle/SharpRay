namespace ProtoCity
{
    public class Edge : Entity
    {
        public PointHandler A { get; }
        public PointHandler B { get; }
        public Vector2 C { get; private set; }
        public int Depth { get; set; } = 50;

        private const float rayLength = 1500f;

        public Edge(PointHandler a, PointHandler b)
        {
            A = a;
            B = b;
            C = Vector2.Lerp(A.Position, B.Position, .5f);
        }

        public override void Render()
        {
            DrawLineEx(A.Position, B.Position, 3, Color.LightGray);

            DrawTextV("A", A.Position, 15, Color.Black);
            DrawTextV("B", B.Position - new Vector2(15, 0), 15, Color.Black);

            DrawRays();
        }

        private void DrawRays()
        {
            var (rayOrigin, rayA, rayB) = GetRays();

            //DrawCircleV(rayOrigin, 3, Color.RED);
            //DrawLineV(rayOrigin, rayA, Color.RED);
            //DrawLineV(rayOrigin, rayB, Color.RED);
        }

        public (Vector2 origin, Vector2 rayA, Vector2 rayB) GetRays()
        {
            C = Vector2.Lerp(A.Position, B.Position, .5f);
            var (n1, n2) = GetNormals();
            var rayOrigin = C + n1;
            var rayA = Vector2.Transform(Vector2.Normalize(A.Position - C), Matrix3x2.CreateScale(rayLength) * Matrix3x2.CreateTranslation(rayOrigin));
            var rayB = Vector2.Transform(Vector2.Normalize(B.Position - C), Matrix3x2.CreateScale(rayLength) * Matrix3x2.CreateTranslation(rayOrigin));
            return (rayOrigin, rayA, rayB);
        }

        private (Vector2 n1, Vector2 n2) GetNormals()
        {
            var d = B.Position - A.Position;
            var n1 = Vector2.Normalize(new Vector2(-d.Y, d.X)) * Depth;
            var n2 = Vector2.Normalize(new Vector2(d.Y, -d.X)) * Depth;
            return (n1, n2);
        }
    }
}
