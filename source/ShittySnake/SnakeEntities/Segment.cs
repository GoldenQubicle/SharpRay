using SharpRay.Components;

namespace SnakeEntities
{
    public enum Direction { Up, Right, Down, Left }

    public class Segment : Entity, IHasUpdate, IHasRender, IHasCollider, IEventEmitter<IGameEvent>
    {
        public Vector2 Bounds { get; init; }
        public Direction Direction { get; set; }
        public Direction NextDirection { get; set; }
        protected bool IntervalElapsed { get; set; }
        protected Vector2 Center { get; set; }
        public Segment Next { get; private set; }
        protected Color Color { get; set; }
        protected Color ScaleColor { get; set; }
        public ICollider Collider { get; set; }

        private static double interval = LocomotionInterval * TickMultiplier;
        private double current = 0d;
        private double prevDistance = 0f;
        protected bool IsDigesting { get; private set; }
        public Action<IGameEvent> EmitEvent { get; set; }

        private bool goNext = false;
        

        public override void Update(double deltaTime)
        {
            current += deltaTime;

            if (current > interval)
            {
                current = 0d;
                prevDistance = 0d;
                IntervalElapsed = true;

                if (IsDigesting && goNext && Next is not null)
                {
                    Next.SetIsDigesting(true);
                    SetIsDigesting(false);
                    goNext = false;
                }

                if (IsDigesting && goNext && Next is null)
                {
                    EmitEvent(new PoopParticleSpawn
                    {
                        Position = Center - new Vector2(CellSize - PoopSize, CellSize - PoopSize) / 2,
                    });
                    SetIsDigesting(false);
                    goNext = false;
                }

                if (IsDigesting && !goNext)
                    goNext = true;
            }

            DoLocomotion();

            //update position used by collider
           Collider.Position = new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2);
        }

        public override void Render()
        {
            Color = IsDigesting ? Brown: DarkPurple;

            DrawRectangleRounded((Collider as RectCollider).Rect, .35f, 1, Color);
            DrawRectangleRoundedLines((Collider as RectCollider).Rect, .35f, 1, Purple);

            var (start, end) = Direction switch
            {
                Direction.Up => (-270, -90),
                Direction.Right => (180, 0),
                Direction.Down => (90, -90),
                Direction.Left => (-180, 0),
            };

            ScaleColor = IsDigesting ? Gold : Yellow;
            DrawCircleSectorLines(Center, SegmentSize/3, start, end, 16, ScaleColor);
        }

        public Segment SetNext()
        {
            Next = new Segment
            {
                Position = Center - Size / 2,
                RenderLayer = RenderLayer,
                Size = new Vector2(SegmentSize, SegmentSize),
                Direction = Direction,
                Bounds = Bounds,
                Center = Direction switch
                {
                    Direction.Up => Center + new Vector2(0f, CellSize),
                    Direction.Right => Center + new Vector2(-CellSize, 0f),
                    Direction.Down => Center + new Vector2(0f, -CellSize),
                    Direction.Left => Center + new Vector2(CellSize, 0f),
                },
                Collider = new RectCollider
                {
                    Position = new Vector2(Center.X - Size.X / 2, Center.Y - Size.Y / 2),
                    Size = new Vector2(SegmentSize, SegmentSize)
                }
            };
            return Next;
        }

        public void SetDirection(Direction nextDirection)
        {
            Next?.SetDirection(Direction);
            Direction = nextDirection;
        }

        public void SetIsDigesting(bool b) => IsDigesting = b;

        private void DoLocomotion()
        {
            var t = Math.Clamp(MapRange(current, 0d, interval, 0d, 1d), 0d, 1d);
            var e = Easings.EaseBackInOut((float)t, 0f, CellSize, 1f);
            var d = e - prevDistance;
            prevDistance = e;

            Center += Direction switch
            {
                Direction.Up => new Vector2(0f, -(float)d),
                Direction.Right => new Vector2((float)d, 0f),
                Direction.Down => new Vector2(0f, (float)d),
                Direction.Left => new Vector2(-(float)d, 0f),
            };
        }
    }
}
