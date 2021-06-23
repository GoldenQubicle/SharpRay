using System;
using System.Collections.Generic;
using System.Numerics;
using static SharpRay.SnakeConfig;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class FoodParticleSpawner : Entity, IEventEmitter<IGameEvent>, IGameEventListener
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        private double rndInterval;
        private double current;
        private readonly Random Random = new ();

        private readonly HashSet<Vector2> OccupiedCells = new();
    
        public void Initialize(int particles)
        {
            rndInterval = Program.MapRange(Random.NextDouble(), 0d, 1d, MinFoodSpawnInterval, MaxFoodSpawnInterval) * Program.TickMultiplier;
            for (var i = 0; i < particles; i++)
                EmitEvent(new FoodParticleSpawn { Position = GetNewFoodParticlePosition() });
        }

        public override void Update(double deltaTime)
        {
            current += deltaTime;
            if (current > rndInterval)
            {
                EmitEvent(new FoodParticleSpawn { Position = GetNewFoodParticlePosition() });
                rndInterval = Program.MapRange(Random.NextDouble(), 0d, 1d, MinFoodSpawnInterval, MaxFoodSpawnInterval) * Program.TickMultiplier;
                current = 0d;
            }
        }

        private Vector2 GetNewFoodParticlePosition()
        {
            var x = GetRandomValue(0, (int)Size.X / CellSize) * CellSize;
            var y = GetRandomValue(0, (int)Size.Y / CellSize) * CellSize;

            while (OccupiedCells.Contains(new Vector2(x, y)))
            {
                x = GetRandomValue(0, (int)Size.X / CellSize) * CellSize;
                y = GetRandomValue(0, (int)Size.Y / CellSize) * CellSize;
            }
            var pos = new Vector2(x, y);
            OccupiedCells.Add(pos);
            return pos;
        }

        public Action<IGameEvent, Entity> OnGameEventAction { get; set; }

        public void OnGameEvent(IGameEvent e)
        {
            if (e is SnakeConsumedFood f) OccupiedCells.Remove(f.FoodParticle.Position);

            if (e is PoopParticleSpawn ps) OccupiedCells.Add(ps.Position);

            if (e is SnakeConsumedPoop p) OccupiedCells.Remove(p.PoopParticle.Position);
        }
    }
}
