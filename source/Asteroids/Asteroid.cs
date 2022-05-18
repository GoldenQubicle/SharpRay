namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider
    {
        public ICollider Collider { get; }
        public Vector2 Heading { get; }
        private Vector2 offset;
        private Vector2 texturePos;
        public int Stage { get; }
        private readonly Texture2D texture;
        private float RotationAngle; //inital orientation
        private float RotationSpeed;// in radians per fixed update
        private Matrix3x2 Translation;

        public Asteroid(Vector2 position, Vector2 heading, int stage, Texture2D texture)
        {
            Position = position;
            Size = new Vector2(texture.width, texture.height);
            Heading = heading;
            Stage = stage;
            offset = Size / 2;
            this.texture = texture;
            Translation = Matrix3x2.CreateTranslation(Heading);
            RotationAngle = GetRandomValue(-50, 50) / 1000f;
            RotationSpeed = GetRandomValue(-50, 50) / 1000f;
            Collider = new RectCollider { Position = Position, Size = Size };
            RenderLayer = Game.RlAsteroidsBullets;
        }

        public override void Update(double deltaTime)
        {
            Position = Vector2.Transform(Position, Translation);
            texturePos = Vector2.Transform(Position - offset, Matrix3x2.CreateRotation(RotationAngle, Position));
            (Collider as RectCollider).Position = Position - offset;
            RotationAngle += RotationSpeed;

            //bounds check
            if (Position.X < 0) Translation = Matrix3x2.CreateTranslation(Heading * new Vector2(1.5f, 0));
            if (Position.X > Game.WindowWidth) Translation = Matrix3x2.CreateTranslation(Heading * new Vector2(-1.5f, 0));
            if (Position.Y < 0) Translation = Matrix3x2.CreateTranslation(Heading * new Vector2(0, -1.5f));
            if (Position.Y > Game.WindowHeight) Translation = Matrix3x2.CreateTranslation(Heading * new Vector2(0, 1.5f));
        }

        public override void Render()
        {
            DrawTextureEx(texture, texturePos, RAD2DEG * RotationAngle, 1f, Color.WHITE);

            //DEBUG
            //Collider.Render();
            //DrawCircleV(Position, 5, Color.DARKGREEN);
        }


        public override void OnMouseEvent(IMouseEvent e)
        {

        }
    }
}
