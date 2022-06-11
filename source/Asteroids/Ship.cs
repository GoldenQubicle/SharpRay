namespace Asteroids
{
    public class Ship : Entity, IHasCollider, IHasCollision, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>, IKeyBoardListener, IMouseListener
    {
        public record Layout(Vector2 Position, int Health);

        public Action<IGameEvent> EmitEvent { get; set; }
        public ICollider Collider { get; }
        public Texture2D ShipTexture { get; set; }
        public Texture2D DamgageTexture { get; set; }
        public bool HasTakenDamage { get; set; }
        public int Health { get; set; } = MaxHealth;

        public const string EngineSound = nameof(EngineSound);
        public const string ThrusterSound = nameof(ThrusterSound);
        public const string HitSound = nameof(HitSound);

        private const string Accelerate = nameof(Accelerate);
        private const string Decelerate = nameof(Decelerate);
        private const string RotateIn = nameof(RotateIn);
        private const string RotateOut = nameof(RotateOut);
        private const string Left = nameof(Left);
        private const string Right = nameof(Right);

        private const float HalfPI = MathF.PI / 2;
        private readonly float radius;
        private Dictionary<string, Easing> Motions;

        private readonly double accelerateTime = 500; // time it takes to reach max acceleration
        private readonly double decelerateTime = 2500; // time it takes from max acceleration to come to a stand still
        private readonly float maxAcceleration = 10;
        private float n_acceleration = 0f; //normalized 0-1
        private float acceleration = 0f;
        private bool hasAcceleration;

        private readonly double rotateInTime = 300; // time it takes to reach max rotation angle
        private readonly double rotateOutTime = 550; // time it takes from max rotation angle to come to a stand still
        private readonly float maxRotation = 3.5f * DEG2RAD; // in radians per frame, essentially
        private float n_rotation = 0f; //normalized 0-1
        private float rotation = 0f;
        private bool hasRotation;
        private string direction;

        private float scale = .75f;
        private readonly Vector2 offset; //used for render position textures

        public Ship(Layout layout, Texture2D texture)
        {
            Position = layout.Position;
            Size = new Vector2(texture.width, texture.height);
            RenderLayer = RlShip;
            ShipTexture = texture;
            offset = new Vector2(texture.width / 2, texture.height / 2) * scale;
            radius = (Size.X / 2) * scale;

            Collider = new CircleCollider
            {
                Center = Position,
                Radius = radius,
                HitPoints = 16
            };

            Health = layout.Health;

            Motions = new Dictionary<string, Easing>
            {
                { Accelerate, new Easing(Easings.EaseQuadOut, accelerateTime) },
                { Decelerate, new Easing(Easings.EaseCircOut, decelerateTime, isReversed: true) },
                { RotateIn,   new Easing(Easings.EaseSineOut, rotateInTime) },
                { RotateOut,  new Easing(Easings.EaseSineIn, rotateOutTime, isReversed: true) },
            };
        }

        public override void Update(double deltaTime)
        {
            if (IsPaused) return;

            //update motions
            foreach (var m in Motions.Values) m.Update(deltaTime);

            //get normalized motion values if applicable
            if (hasAcceleration)
                n_acceleration = Motions[Accelerate].GetValue();
            else if (n_acceleration > 0)
                n_acceleration = Motions[Decelerate].GetValue();

            if (hasRotation)
                n_rotation = Motions[RotateIn].GetValue();
            else if (n_rotation > 0)
                n_rotation = Motions[RotateOut].GetValue();

            //update rotation
            var r = n_rotation * maxRotation;
            rotation += direction == Left ? -1 * r : r;

            //update & apply acceleration to position
            acceleration = n_acceleration * maxAcceleration;
            Position += new Vector2(MathF.Cos(rotation - HalfPI) * acceleration, MathF.Sin(rotation - HalfPI) * acceleration);

            //dont forget to update collider
            (Collider as CircleCollider).Center = Position;

            //bounds check
            if (Position.X < 0) Position = new Vector2(WindowWidth, Position.Y);
            if (Position.X > WindowWidth) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, WindowHeight);
            if (Position.Y > WindowHeight) Position = new Vector2(Position.X, 0);

            //update sounds
            if (!IsSoundPlaying(Sounds[EngineSound])) PlaySound(EngineSound);
            if (!IsSoundPlaying(Sounds[ThrusterSound])) PlaySound(ThrusterSound);

            //TODO: want to set overall sound fx from within game
            SetSoundVolume(Sounds[EngineSound], n_acceleration * .5f);
            SetSoundVolume(Sounds[ThrusterSound], n_rotation * .5f);
        }

        public void OnCollision(IHasCollider e)
        {
            if (e is Asteroid a)
            {
                HasTakenDamage = true;
                Health -= Asteroid.GetDamageDone(a.Definition);
                //EmitEvent(new ShipHitAsteroid
                //{
                //    LifeLost = Health <= 0,
                //    LifeIconIdx = PlayerLifes,
                //    ShipHealth = Health,
                //    Asteroid = a,
                //});
            }

            if (e is PickUp p)
            {
                p.OnPickUp();
                EmitEvent(new ShipPickUp
                {
                    PickUp = p
                });
            }
        }

        public override void Render()
        {
            var texPos = Vector2.Transform(Position - offset, Matrix3x2.CreateRotation(rotation, Position));
            DrawTextureEx(ShipTexture, texPos, RAD2DEG * rotation, scale, Color.WHITE);

            if (HasTakenDamage)
                DrawTextureEx(DamgageTexture, texPos, RAD2DEG * rotation, scale, Color.DARKGRAY);

            //Collider.Render();
            DrawCircleV(Position, 5, Color.PINK);
            var start = Position + new Vector2(0, WindowWidth);
            var end = Position + new Vector2(0, -WindowWidth);
            var r = Matrix3x2.CreateRotation(180 * DEG2RAD + rotation, Position);
            start = Vector2.Transform(start, r);
            end = Vector2.Transform(end, r);
            DrawLineV(start, end, Color.YELLOW);


            var mp = GetMousePosition();
            DrawLineV(Position, mp, Color.GREEN);
            var a = AngleBetween(start, Position, mp);
            DrawTextV($"{a * RAD2DEG}", Position + Vector2.One * 5, 24, Color.RED);

            //DrawCircleV(texPos, 5, Color.DARKPURPLE);
        }

        public static double AngleBetween(Vector2 vector1, Vector2 vector2, Vector2 vector3)
        {
            var a = vector1 - vector2;
            var b = vector3 - vector2;

            return Math.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (e is MouseMovement mm)
            {
                var start = Position + new Vector2(0, WindowWidth);
                var end = Position + new Vector2(0, -WindowWidth);
                var r = Matrix3x2.CreateRotation(180 * DEG2RAD + rotation, Position);
                start = Vector2.Transform(start, r);
                end = Vector2.Transform(end, r);
                var sign = ((end.X - start.X) * (mm.Position.Y - start.Y) -
                    (end.Y - start.Y) * (mm.Position.X - start.X));

                var a = AngleBetween(start, Position, GetMousePosition()) * RAD2DEG;

                (hasRotation, direction) = sign switch
                {
                    > 0 when !hasRotation && a > 45 => StartRotateIn(Left),
                    < 0 when !hasRotation && a > 45 => StartRotateIn(Right),
                    //KeyLeftReleased or KeyRightReleased => StartRotateOut(),
                    _ => StartRotateOut()
                };
            }
        }

        public override void OnKeyBoardEvent(IKeyBoardEvent e)
        {
            (hasRotation, direction) = e switch
            {
                KeyLeftDown when !hasRotation => StartRotateIn(Left),
                KeyRightDown when !hasRotation => StartRotateIn(Right),
                KeyLeftReleased or KeyRightReleased => StartRotateOut(),
                _ => (hasRotation, direction)
            };

            hasAcceleration = e switch
            {
                KeyUpDown when !hasAcceleration => StartAccelerate(),
                KeyUpReleased when hasAcceleration => StartDecelerate(),
                _ => hasAcceleration
            };

            //obvisouly this will change w primary & secondary weapon
            if (e is KeySpaceBarPressed)
                PrimaryWeapon.Fire(new ShipFiredBullet
                {
                    Origin = Position + new Vector2(MathF.Cos(rotation - HalfPI) * radius, MathF.Sin(rotation - HalfPI) * radius),
                    Angle = rotation - HalfPI,
                    Force = acceleration
                });
        }

        private (bool, string) StartRotateOut()
        {
            Motions[RotateOut].SetElapsedTime(n_rotation);
            return (false, direction);
        }

        private (bool, string) StartRotateIn(string direction)
        {
            Motions[RotateIn].SetElapsedTime(n_rotation);
            return (true, direction);
        }

        private bool StartAccelerate()
        {
            Motions[Accelerate].SetElapsedTime(n_acceleration);
            return true;
        }

        private bool StartDecelerate()
        {
            Motions[Decelerate].SetElapsedTime(n_acceleration);
            return false;
        }
    }
}
