namespace Asteroids
{
    public static class PrimaryWeapon
    {
        public const string SingleSound = nameof(SingleSound);
        public const string TripleSound = nameof(TripleSound);
        public const string QuintupleSound = nameof(QuintupleSound);
        public enum Mode
        {
            Single,
            Triple,
            Quintuple,
        }

        public record State(Mode Mode, Bullet.Type BulletType, float BulletSpeed, float BulletLifeTime, int AmmoCount);
        
        private static State BeginState = new State(Mode.Single, Bullet.Type.Simple, 10f, 500f, -1);
        private static State CurrentState { get; set; }

        private static Stack<State> _states = new();

        public static int GetStatesCount() => _states.Count;

        public static void OnStartLevel()
        {
            _states.Clear();
            _states.Push(CurrentState);
            CurrentState = _states.Peek();
        }

        public static void ChangeBulletLifeTime(float multiplier) =>
            OnChangeState(CurrentState with { BulletLifeTime = CurrentState.BulletLifeTime * multiplier });

        public static void ChangeMode(Mode mode) =>
            OnChangeState(CurrentState with { Mode = mode });

        public static void ChangeBulletType(Bullet.Type type) =>
            OnChangeState(CurrentState with { BulletType = type });

        public static void OnLifeLost()
        {
            var count = _states.Count - 1;
            for (var i = 0; i < count; i++)
            {
                _states.Pop();
                CurrentState = _states.Peek();
            }
        }

        public static void OnGameStart(Mode mode = Mode.Single, Bullet.Type type = Bullet.Type.Simple) =>
            CurrentState = BeginState with {Mode = mode, BulletType = type };

        private static void OnChangeState(State state)
        {
            _states.Push(state);
            CurrentState = _states.Peek();
        }

        public static void Fire(ShipFiredBullet sfb)
        {
            foreach (var b in CreateBullets(sfb))
            {
                AddEntity(b, Game.OnGameEvent);
            }

            var soundKey = CurrentState.Mode switch
            {
                Mode.Single => SingleSound,
                Mode.Triple  => TripleSound,
                Mode.Quintuple => QuintupleSound
            };
            SetSoundPitch(Sounds[soundKey], GetRandomValue(92, 108) / 100f);
            PlaySound(soundKey);
        }

        private static List<Bullet> CreateBullets(ShipFiredBullet sfb) => CurrentState.Mode switch
        {
            Mode.Single => new()
            {
                new(Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, CurrentState))
            },
            Mode.Triple => new()
            {
                new(Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 10f, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 10f, sfb.Force,  CurrentState)),
            },
            Mode.Quintuple => new()
            {
                new(Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 15f, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 15f, sfb.Force,  CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 30f, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 30f, sfb.Force,  CurrentState)),
            },
            _ => throw new NotImplementedException()
        };
    }
}
