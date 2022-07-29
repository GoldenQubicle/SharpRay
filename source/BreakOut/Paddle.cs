namespace BreakOut
{
    internal class Paddle : Entity, IHasRender, IMouseListener, IKeyBoardListener, IHasCollider
    {
        public record struct PaddleState(bool IsMoving, float Speed);
        public PaddleState State { get; private set; }
        public Vector2 Speed { get; private set; } = new Vector2(.25f, 0);

        public ICollider Collider { get; set; }
        private float roundness = 0f;
        public Paddle()
        {
            Position = new Vector2(WindowWidth / 2, WindowHeight / 2);
            Size = new Vector2(100, 100);
            Collider = new RectCollider
            {
                Position = Position - Size / 2,
                Size = Size
            };
        }

        public override void Render()
        {
            var rect = new Raylib_cs.Rectangle
            {
                width = Size.X,
                height = Size.Y,
                x = Position.X - Size.X / 2,
                y = Position.Y - Size.Y / 2,
            };


            DrawRectangleRounded(rect, roundness, 5, Color.YELLOW);
            DrawRectangleRoundedLines(rect, roundness, 5, 2, Color.GOLD);
            DrawCircleV(Position, 2, Color.BLACK);

            (Collider as RectCollider).Position = Position - Size / 2;
            Collider.Render();

            State = new PaddleState(false, 0);
        }
        // TODO rework updating paddle position inside of actual update
        // i.e. mouse & key events should only inform us of movement, not set the position themselves!
        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseMovement mm)
            {
                State = new PaddleState(true, mm.Position.X - mm.PreviousPosition.X);
                Position = new Vector2(mm.Position.X, Position.Y);
            }

            Print(State.IsMoving);
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyRightDown && Position.X < WindowWidth)
            {
                State = new PaddleState(true, Speed.X);
                Position += Speed;
            }
            if (e is KeyLeftDown && Position.X > 0)
            {
                State = new PaddleState(true, -Speed.X);
                Position -= Speed;
            }
        }


    }
}
