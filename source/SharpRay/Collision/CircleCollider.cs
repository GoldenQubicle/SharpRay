namespace SharpRay.Collision
{
    public class CircleCollider : Collider
    {
        public Vector2 Center { get; set; }
        public float Radius { get; init; }
        public int HitPoints { get; init; } = 4; //number of point on circumference used in collision detection with rect pro collider. 

        public IEnumerable<(Vector2 v, int idx )> GetHitPoints()
        {
            var theta = MathF.Tau / HitPoints;

            for (var i = 0; i < HitPoints; i++)
            {
                yield return (Center + new Vector2(MathF.Cos(i * theta) * Radius, MathF.Sin(i * theta) * Radius), i);
            }
        }

        public override void Render()
        {
            foreach (var p in GetHitPoints())
            {
                DrawCircleV(p.v, 2, RED);
                DrawText(p.idx.ToString(), (int) p.v.X, (int)p.v.Y, 10, BLACK);
            }

            DrawCircleLines((int)Center.X, (int)Center.Y, Radius, Color);
        }
    }
}
