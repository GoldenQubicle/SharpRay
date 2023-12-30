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
                X = Position.X,
                Y = Position.Y,
                Width = Size.X,
                Height = Size.Y
            };
        }
        public override void Render() => DrawRectangleLinesEx(Rect, 2, Color);
        
    }
}
