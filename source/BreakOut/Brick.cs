namespace BreakOut
{
    public class Brick: Entity, IHasRender, IHasCollider
    {
        public enum Type
        {
            Simple,
        }

        public ICollider Collider { get; }
        public Type BrickType { get; }
        private int HitPoints { get; set; }
        public Action<IGameEvent> EmitEvent { get ; set ; }

        public Brick(Vector2 position)
        {
            BrickType = Type.Simple;
            HitPoints = GetHitPoints();
            Size = new Vector2(100, 20);
            Position = position;
            Collider = new RectCollider
            {
                Position = Position,
                Size = Size
            };
        }

        public override void Render()
        {
            
            DrawRectangleGradientEx((Collider as RectCollider).Rect, Color.YELLOW, Color.PURPLE, Color.ORANGE, Color.LIME);
            DrawTextV(HitPoints.ToString(), Position, 10, Color.VIOLET);
            //Collider.Render();

        }

        public void OnBounce(Ball ball)
        {
            HitPoints -= ball.GetHitPoints();

            if(HitPoints == 0)
            {
                RemoveEntity(this);
            }
        }

        public int GetHitPoints() => BrickType switch
        {
            Type.Simple => 5,
            _ => -1
        };
    }
}
