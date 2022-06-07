namespace Asteroids
{
    public static class PrimaryWeapon
    {
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
            _states.Push(new State(Mode.Single, Bullet.Type.Simple, 10f, 750f, -1));
            CurrentState = _states.Peek();
        }

        public static void ChangeMode(Mode mode) =>
            OnChangeState(CurrentState with { Mode = mode });

        public static void ChangeBulletType(Bullet.Type type) =>
            OnChangeState(CurrentState with { BulletType = type });

        public static void ChangeBulletType(Bullet.Type type, int ammo) =>
            OnChangeState(CurrentState with { BulletType = type, AmmoCount = ammo });

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

            if (CurrentState.AmmoCount > 0)
            {
                CurrentState = CurrentState with { AmmoCount = CurrentState.AmmoCount - 1 };
            }
            //NOTE this is not correct, as we cannot assume the previous state has the correct data
            //instead we want to get back to previous bullet type in this use case
            if (CurrentState.AmmoCount == 1)
            {
                CurrentState = _states.Pop() with { Mode = CurrentState.Mode };
            }
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
