using System;

namespace SharpRay
{
    public class ParticleSpawner : Entity, IEventEmitter<IGameEvent>
    {
        public Action<IGameEvent> EmitEvent { get; set; }

        private double rndInterval;
        private double current;
        private Random Random = new Random();
        private double min = 500d;
        private double max = 750d;

        public ParticleSpawner()
        {
            rndInterval = Program.MapRange(Random.NextDouble(), 0d, 1d, min, max) * Program.TickMultiplier;
        }

        public override void Update(double deltaTime)
        {
            current += deltaTime;
            if (current > rndInterval)
            {
                EmitEvent(new FoodParticleSpawn());
                rndInterval = Program.MapRange(Random.NextDouble(), 0d, 1d, min, max) * Program.TickMultiplier;
                //rndInterval = double.MaxValue;
                current = 0d;
            }
        }
    }
}
