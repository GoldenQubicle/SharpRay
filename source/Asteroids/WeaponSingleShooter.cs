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
            var data = Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, Bullet.Type.Simple);

            AddEntity(new Bullet(data), Game.OnGameEvent);
        }
    }

    public class WeaponTripleShooter : IPrimaryWeapon
    {
        public void Fire(ShipFiredBullet sfb)
        {
            var data1 = Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, Bullet.Type.Simple);
            var data2 = Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 15f, sfb.Force, Bullet.Type.Simple);
            var data3 = Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 15f, sfb.Force, Bullet.Type.Simple);

            AddEntity(new Bullet(data1), Game.OnGameEvent);
            AddEntity(new Bullet(data2), Game.OnGameEvent);
            AddEntity(new Bullet(data3), Game.OnGameEvent);
        }
    }

}
