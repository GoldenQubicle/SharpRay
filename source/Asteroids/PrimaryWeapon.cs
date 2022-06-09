namespace Asteroids
{
    public static class PrimaryWeapon
    {
        public const string SingleSound = nameof(SingleSound);
        public const string TripleSound = nameof(TripleSound);
        public enum Mode
        {
            Single,
            TripleNarrow,
            TripleWide,
        }

        public record State(Mode Mode, Bullet.Type BulletType, float BulletSpeed, float BulletLifeTime, int AmmoCount);

        private static State CurrentState { get; set; }

        private static Stack<State> _states = new();

        public static void OnStartGame()
        {
            _states.Push(new State(Mode.Single, Bullet.Type.Simple, 10f, 500f, -1));
            CurrentState = _states.Peek();
        }

        public static void ChangeBulletLifeTime(float multiplier) =>
               OnChangeState(CurrentState with { BulletLifeTime = CurrentState.BulletLifeTime * multiplier });

        public static void ChangeMode(Mode mode) =>
            OnChangeState(CurrentState with { Mode = mode });

        public static void ChangeBulletType(Bullet.Type type) =>
            OnChangeState(CurrentState with { BulletType = type });


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
                Mode.TripleWide or Mode.TripleNarrow => TripleSound
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
            Mode.TripleNarrow => new()
            {
                new(Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 10f, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 10f, sfb.Force,  CurrentState)),
            },
            Mode.TripleWide => new()
            {
                new(Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 25f, sfb.Force, CurrentState)),
                new(Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 25f, sfb.Force,  CurrentState)),
            },
            _ => throw new NotImplementedException()
        };
    }
}
