namespace Asteroids
{
    public class PickUp : Entity, IHasCollider, IHasRender
    {
        public const string PickupSound = nameof(PickupSound);
        public const string SpawnSound = nameof(SpawnSound);
        public ICollider Collider { get; }
        public Action<Ship> OnPickUp { get; init; }
        public string Description { get; init; }
        public int SpawnScore { get; init; }
        public bool HasSpawned { get; set; }

        public PickUp()
        {
            Size = new Vector2(25, 25);
            Collider = new RectCollider
            {
                Size = Size
            };

            RenderLayer = RlPickUps;
        }

        public void OnSpawn(Vector2 pos)
        {
            Position = pos;
            (Collider as RectCollider).Position = pos;
            AddEntity(this);
            PlaySound(Sounds[SpawnSound]);
            HasSpawned = true;
        }

        public override void Render()
        {
            Collider.Render();
            DrawRectangleV(Position, Size, Color.YELLOW);
        }
    }
}
