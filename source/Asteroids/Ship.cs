namespace Asteroids
{
    public class Ship : Entity, IHasCollider, IHasCollision, IHasRender, IHasUpdate, IEventEmitter<IGameEvent>, IKeyBoardListener
    {
        public Action<IGameEvent> EmitEvent { get; set; }
        public ICollider Collider { get; }
        public Texture2D ShipTexture { get; set; }
        public Texture2D DamgageTexture { get; set; }
        public bool HasTakenDamage { get; set; }

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
        private double resetInterval = 500 * SharpRayConfig.TickMultiplier;
        private double resetTimer;

        public Ship(Vector2 position, int health, Texture2D texture)
        {
            Position = position;
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

            Motions = new Dictionary<string, Easing>
            {
                { Accelerate, new Easing(Easings.EaseQuadOut, accelerateTime) },
                { Decelerate, new Easing(Easings.EaseCircOut, decelerateTime, isReversed: true) },
                { RotateIn,   new Easing(Easings.EaseSineOut, rotateInTime) },
                { RotateOut,  new Easing(Easings.EaseSineIn, rotateOutTime, isReversed: true) },
            };
        }

        public void Reset()
        {
            HasTakenDamage = false; // prevent damage texture from being visible
            CurrentHealth = MaxHealth;
            Position = new Vector2(WindowWidth / 2, WindowHeight / 2);
            rotation = 0;
            n_rotation = 0;
            acceleration = 0;
            n_acceleration = 0;
            resetTimer = 0;
        }

        public override void Update(double deltaTime)
        {
            if (IsPaused) return;
            resetTimer += deltaTime;

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
            var r = (n_rotation * maxRotation);
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
            if (resetTimer < resetInterval) return;

            if (e is Asteroid a)
            {
                HasTakenDamage = true;
                CurrentHealth -= Asteroid.GetDamageDone(a.Definition);
                EmitEvent(new ShipHitAsteroid
                {
                    Asteroid = a,
                });

                if (CurrentHealth <= 0)
                {
                    EmitEvent(new ShipLifeLost
                    {
                        LifeIconIdx = CurrentLifes,
                    });
                }
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
            //DrawCircleV(Position, 5, Color.PINK);
            //DrawCircleV(tp, 5, Color.DARKPURPLE);
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
