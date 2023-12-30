namespace SharpRay.Collision
{
    public class CircleCollider : Collider
    {
        public float Radius { get; init; }
        public int HitPoints { get; init; } = 4; //number of point on circumference used in collision detection with rect pro collider. 

        public IEnumerable<Vector2> GetHitPoints()
        {
            var theta = MathF.Tau / HitPoints;

            for (var i = 0; i < HitPoints; i++)
            {
                yield return Position + new Vector2(MathF.Cos(i * theta) * Radius, MathF.Sin(i * theta) * Radius);
            }
        }

        public override void Render()
        {
            foreach (var p in GetHitPoints())
                DrawCircleV(p, 2, RED);

            DrawCircleLines((int)Position.X, (int)Position.Y, Radius, Color);
        }
    }
}
