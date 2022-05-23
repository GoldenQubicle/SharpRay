namespace Asteroids
{
    public class Asteroid : GameEntity, IHasCollider
    {
        public ICollider Collider { get; }
        public Vector2 Heading { get; private set; }
        private Vector2 offset;
        private Vector2 texturePos;
        public int Stage { get; }
        private readonly Texture2D texture;
        private float RotationAngle; //inital orientation
        private float RotationSpeed;// in radians per fixed update
        private bool HasSpawned;
        public Asteroid(Vector2 position, Vector2 heading, int stage, Texture2D texture)
        {
            Position = position;
            Size = new Vector2(texture.width, texture.height);
            Heading = heading;
            Stage = stage;
            offset = Size / 2;
            this.texture = texture;
            RotationAngle = GetRandomValue(-50, 50) / 1000f;
            RotationSpeed = GetRandomValue(-50, 50) / 1000f;
            Collider = new RectCollider { Position = Position, Size = Size };
            RenderLayer = Game.RlAsteroidsBullets;
        }

        public override void Update(double deltaTime)
        {
            Position += Heading;
            texturePos = Vector2.Transform(Position - offset, Matrix3x2.CreateRotation(RotationAngle, Position));
            (Collider as RectCollider).Position = Position - offset;
            RotationAngle += RotationSpeed;


            Heading = Position switch
            {
                Vector2 { X: < 0 } or Vector2 { X: > Game.WindowWidth } when HasSpawned => Vector2.Reflect(Heading, Vector2.UnitX) * new Vector2(1.15f, 1.15f),
                Vector2 { Y: < 0 } or Vector2 { Y: > Game.WindowHeight } when HasSpawned => Vector2.Reflect(Heading, Vector2.UnitY) * new Vector2(1.15f, 1.15f),
                _ => Heading
            };

            if (!HasSpawned
                && Position.X > 0 && Position.X < Game.WindowWidth
                && Position.Y > 0 && Position.Y < Game.WindowHeight)
            {
                HasSpawned = true;
            }
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
