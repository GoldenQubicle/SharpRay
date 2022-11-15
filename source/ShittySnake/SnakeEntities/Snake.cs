namespace SnakeEntities
{
    public class Snake : Segment, IHasCollision
    {
        private Func<SnakeConsumedFood> OnConsumedFood { get; set; }
        private Func<DespawnPoop> OnConsumedPoop { get; set; }
        private Color HeadColor { get; set; }
        private int segmentCount = 3;
        public Snake(Vector2 position)
        {
            Position = position;
            Size = new Vector2(HeadSize, HeadSize);
            Center = new Vector2(Position.X + CellSize / 2, Position.Y + CellSize / 2);
            Collider = new RectCollider
            {
                Position = position,
                Size = Size,
            };
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is ParticleFood f)
            {
                SetIsDigesting(true);

                var last = Next;
                while (last.Next is not null)
                    last = last.Next;

                OnConsumedFood = () =>
                {
                    segmentCount++;

                    return new SnakeConsumedFood
                    {
                        FoodParticle = f,
                        NextSegment = last.SetNext()
                    };
                };
            }

            //ignore first segment collision due to locomotion
            if (e is Segment s && s != Next || e is ParticlePoop p)
                EmitEvent(new SnakeGameOver { Score = segmentCount });

        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (IntervalElapsed)
            {
                Next?.SetDirection(Direction);
                Direction = NextDirection;

                EmitEvent(new SnakeLocomotion { Direction = Direction, Position = Position });

                if (OnConsumedFood is not null)
                {
                    EmitEvent(OnConsumedFood());
                    OnConsumedFood = null;
                    SetIsDigesting(false);
                    Next.SetIsDigesting(true);
                }

                if (OnConsumedPoop is not null)
                {
                    EmitEvent(OnConsumedPoop());
                    OnConsumedPoop = null;
                }

                IntervalElapsed = false;
            }

            if (Center.X >= Bounds.X || Center.X <= 0 || Center.Y >= Bounds.Y || Center.Y <= 0)
                EmitEvent(new SnakeGameOver { Score = segmentCount });
        }

        public override void Render()
        {
            HeadColor = IsDigesting ? PINK : MAGENTA; 
            DrawRectangleRounded((Collider as RectCollider).Rect, .5f, 10, HeadColor);
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .5f, 10, 1, PURPLE);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e) =>
            NextDirection = e switch
            {
                KeyUpDown => Direction.Up,
                KeyRightDown => Direction.Right,
                KeyDownDown => Direction.Down,
                KeyLeftDown => Direction.Left,
                _ => NextDirection
            };
    }
}
