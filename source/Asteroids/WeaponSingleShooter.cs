namespace Asteroids
{
    public interface IPrimaryWeapon
    {
        Bullet.Type BulletType { get; set; }
        void Fire(ShipFiredBullet info);
    }

    public class WeaponSingleShooter : IPrimaryWeapon
    {
        public Bullet.Type BulletType { get; set; }

        public void Fire(ShipFiredBullet sfb)
        {
            var data = Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, BulletType);

            AddEntity(new Bullet(data), Game.OnGameEvent);
        }
    }

    public class WeaponTripleShooter : IPrimaryWeapon
    {
        public Bullet.Type BulletType { get; set; }

        public void Fire(ShipFiredBullet sfb)
        {
            var data1 = Bullet.GetData(sfb.Origin, sfb.Angle, sfb.Force, BulletType);
            var data2 = Bullet.GetData(sfb.Origin, sfb.Angle - DEG2RAD * 15f, sfb.Force, BulletType);
            var data3 = Bullet.GetData(sfb.Origin, sfb.Angle + DEG2RAD * 15f, sfb.Force, BulletType);

            AddEntity(new Bullet(data1), Game.OnGameEvent);
            AddEntity(new Bullet(data2), Game.OnGameEvent);
            AddEntity(new Bullet(data3), Game.OnGameEvent);
        }
    }

}
