namespace Asteroids
{
    public interface IPrimaryWeapon
    {
        void Fire(ShipFiredBullet info);
    }

    public class WeaponSingleShooter : IPrimaryWeapon
    {
        public void Fire(ShipFiredBullet sfb)
        {
            AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), Game.OnGameEvent);
        }
    }

    public class WeaponTripleShooter : IPrimaryWeapon
    {
        public void Fire(ShipFiredBullet sfb)
        {
            AddEntity(new Bullet(sfb.Origin, sfb.Angle, sfb.Force), Game.OnGameEvent);
            AddEntity(new Bullet(sfb.Origin, sfb.Angle - DEG2RAD * 15f, sfb.Force), Game.OnGameEvent);
            AddEntity(new Bullet(sfb.Origin, sfb.Angle + DEG2RAD * 15f, sfb.Force), Game.OnGameEvent);
        }
    }

}
