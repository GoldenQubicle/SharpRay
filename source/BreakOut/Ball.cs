namespace BreakOut
{
    public class Ball : Entity, IHasRender, IHasUpdate, IHasCollider, IHasCollision
    {
        public enum Type
        {
            Simple
        }
        public Type BallType { get; }

        public ICollider Collider { get; private set; }
        private Vector2 Heading { get; set; } = new Vector2(6, 4);
        private const float Radius = 8f;
        private bool hasColided;


        public Ball()
        {
            BallType = Type.Simple;
            Position = new Vector2(20, 20);
            Collider = new CircleCollider
            {
                Center = Position,
                Radius = Radius,
                HitPoints = 8
            };
        }

        public void OnCollision(IHasCollider e)
        {
            if(e is Brick b)
            {
                DoBounce(b);
                b.OnBounce(this);
            }

            if (e is Paddle p && !hasColided)
            {
                DoBounce(p);

                /* 
                 *  TODO take the paddle movement into consideration.
                 *  Otherwise if the paddle direction is the same as the ball heading,
                 *  the ball will be inside of the paddle next frame, flip its heading again and travel outwards all in one frame. 
                 *  Giving the illusion of a teleporting ball going through the paddle. 
                 */

            }
        }

        private void DoBounce(IHasCollider p)
        {
            hasColided = true;

            var isInXRange = Position.X > (p.Position - p.Size / 2).X && Position.X < (p.Position + p.Size / 2).X;
            var isAbove = isInXRange && Position.Y < p.Position.Y;
            var isBelow = isInXRange && Position.Y > p.Position.Y;
            var isLeft = Position.X < p.Position.X;
            var isRight = Position.X > p.Position.X;

            Print($"Bal is above : {isAbove} Bal is below : {isBelow}");
            if (isAbove || isBelow) Heading = Vector2.Reflect(Heading, Vector2.UnitY);
            else Heading = Vector2.Reflect(Heading, Vector2.UnitX);

            // Make sure the ball is no longer colliding next frame.
            // Otherwise the heading is flipped again and the ball will 'stick and wiggle' along the edge of the paddle. 
            while (p.Collider.Overlaps(Collider))
            {
                Position += Heading;
                (Collider as CircleCollider).Center = Position;
            }
        }

        public override void Render()
        {
            DrawCircleV(Position, Radius, Color.RAYWHITE);
            //Collider.Render();
        }

        public override void Update(double deltaTime)
        {
            Heading = Position switch
            {
                Vector2 { X: < 0 + Radius } or Vector2 { X: > WindowWidth - Radius } => (Vector2.Reflect(Heading, Vector2.UnitX)),
                Vector2 { Y: < 0 + Radius } or Vector2 { Y: > WindowHeight - Radius } => (Vector2.Reflect(Heading, Vector2.UnitY)),
                _ => Heading
            };

            Position += Heading;
            (Collider as CircleCollider).Center = Position;

            if (hasColided) hasColided = false;
        }

        public int GetHitPoints() => BallType switch
        {
            Type.Simple => 1,
            _ => -1
        };
    }
}
