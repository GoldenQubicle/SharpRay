namespace BreakOut
{
    internal class Paddle : Entity, IHasRender, IMouseListener, IKeyBoardListener, IHasCollider
    {
        public ICollider Collider { get; set; }
        private float roundness = 0f;
        private float speed = .25f;
        public float Momentum { get; private set; }

        public Paddle()
        {
            Position = new Vector2(WindowWidth / 2, WindowHeight / 2);
            Size = new Vector2(100, 20);
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
            //Collider.Render();

        }
        public override void Update(double deltaTime)
        {
            if (Momentum > 0 || Momentum < 0)
            {
                Position += new Vector2(Momentum, 0);
                Momentum = 0;
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseMovement mm)
            {
                Momentum += mm.Position.X - mm.PreviousPosition.X;
            }
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            if (e is KeyRightDown && Position.X < WindowWidth)
            {
                Momentum += speed;
            }

            if (e is KeyLeftDown && Position.X > 0)
            {
                Momentum -= speed;
            }
        }
    }
}
