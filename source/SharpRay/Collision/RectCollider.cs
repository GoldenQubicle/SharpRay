namespace SharpRay.Collision
{
    /// <summary>
    /// Basic rectangle collider. Cannot be rotated. 
    /// </summary>
    public class RectCollider : Collider
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Raylib_cs.Rectangle Rect
        {
            get => new()
            {
                x = Position.X,
                y = Position.Y,
                width = Size.X,
                height = Size.Y
            };
        }
        public override void Render() => DrawRectangleLinesEx(Rect, 2, Color);
        
    }
}
