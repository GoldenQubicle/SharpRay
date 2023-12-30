namespace GardenOfCards.GameData
{
    internal record SoilRenderData(
        Vector2 Center, 
        Vector2[] Points, 
        Vector2[] UV,
        Texture2D Texture,
        Color Color);
    
}
