using System;
using System.Collections.Generic;
using System.Numerics;
using static SharpRay.SnakeConfig;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class FoodParticleSpawner : Entity, IEventEmitter<IGameEvent>, IGameEventListener<FoodParticleSpawner>
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        private double rndInterval;
        private double current;
        private readonly Random Random = new();

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
            var x = GetRandomValue(0, (int)Size.X / CellSize) * CellSize + (CellSize - FoodSize) / 2;
            var y = GetRandomValue(0, (int)Size.Y / CellSize) * CellSize + (CellSize - FoodSize) / 2;

            while (OccupiedCells.Contains(new Vector2(x, y)))
            {
                x = GetRandomValue(0, (int)Size.X / CellSize) * CellSize + (CellSize - FoodSize) / 2;
                y = GetRandomValue(0, (int)Size.Y / CellSize) * CellSize + (CellSize - FoodSize) / 2;
            }
            var pos = new Vector2(x, y);
            OccupiedCells.Add(pos);
            return pos;
        }

        public Action<IGameEvent, FoodParticleSpawner> OnGameEventAction { get; set; }

        public void OnGameEvent(IGameEvent e)
        {
            if (e is SnakeConsumedFood f) OccupiedCells.Remove(f.FoodParticle.Position);

            if (e is PoopParticleSpawn ps) OccupiedCells.Add(ps.Position);

            if (e is DespawnPoop p) OccupiedCells.Remove(p.PoopParticle.Position);
        }
    }
}
