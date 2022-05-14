using Raylib_cs;
using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Numerics;
using static SharpRay.Core.Application;
using static Raylib_cs.Raylib;

namespace Asteroids
{
    public class AsteroidGenerator : GameEntity
    {
        private static string Large = "big";
        private static string Medium = "med";
        private static string Small = "small";
        private static string Tiny = "tiny";
        private const string Brown = nameof(Brown);
        private const string Grey = nameof(Grey);

        public AsteroidGenerator()
        {
            var theta = MathF.Tau / 10;
            for (var i = 0; i < 10; i++)
            {
                var x = MathF.Cos(i * theta) * radius;
                var y = MathF.Sin(i * theta) * radius;
                var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);
                AddEntity(new Asteroid(pos, Vector2.Zero, 3, GetRandomAsteroidTexture(Medium)));
            }
        }

        public static void OnGameEvent(IGameEvent e)
        {
            if (e is AsteroidHitByWeapon ahw)
            {
                if (ahw.Asteroid.Stage > 1)
                {
                    var stage = ahw.Asteroid.Stage - 1;
                    var size = stage == 3 ? Medium : stage == 2 ? Small : Tiny;
                    var amount = stage == 3 ? 7 : stage == 2 ? 5 : 3;
                    for (var i = 1; i <= amount; i++)
                    {
                        var angle = (MathF.Tau / amount) * i;
                        var heading = ahw.Asteroid.Heading + new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                        AddEntity(new Asteroid(ahw.Asteroid.Position, heading, stage, GetRandomAsteroidTexture(size)), OnGameEvent);
                    }
                }

                RemoveEntity(ahw.Asteroid);
            }
        }

        private float radius = Game.WindowWidth/3;

        public override void Render()
        {
            var theta = MathF.Tau / 10;
            for(var i = 0; i < 10; i++)
            {
                var x = MathF.Cos(i * theta) * radius;
                var y = MathF.Sin(i * theta) * radius;
                var pos = new Vector2(x, y) + new Vector2(Game.WindowWidth / 2, Game.WindowHeight / 2);
                DrawCircleV(pos, 5, Color.BLUE);
            }
        }

        private static Texture2D GetRandomAsteroidTexture(string size) =>
            GetTexture2D(Game.meteors[PickAsteroidColor()][size][PickAsteroidVariation(size)]);

        private static string PickAsteroidColor() =>
            GetRandomValue(0, 1) == 1 ? Grey : Brown;

        private static int PickAsteroidVariation(string size) =>
            size.Equals(Large) ? GetRandomValue(1, 4) : GetRandomValue(1, 2);
    }
}
