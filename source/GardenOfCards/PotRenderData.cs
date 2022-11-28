namespace GardenOfCards
{
    [Serializable]
    internal record PotData(
        int nSlots = 2,
        float BasinHeightFactor = 1.75f,
        int BasinSlant = 30,
        int BasinThickness = 5,
        int RimThickness = 50
        );

    internal record PotRenderData(
        float Height,
        float Width,
        Vector2 RimStart,
        Vector2 RimEnd,
        int RimThickness,
        Color RimColor,
        Vector2 BasinLeftUp,
        Vector2 BasinLeftDown,
        Vector2 BasinRightDown,
        Vector2 BasinRightUp,
        int BasinThickness,
        Color BasinColor,
        Vector2 SlotOffset)
    {
        public PotRenderData ApplyOffset(Vector2 offset) => this with
        {
            RimStart = RimStart + offset,
            RimEnd = RimEnd + offset,
            BasinLeftUp = BasinLeftUp + offset,
            BasinLeftDown = BasinLeftDown + offset,
            BasinRightDown = BasinRightDown + offset,
            BasinRightUp = BasinRightUp + offset,
            SlotOffset = SlotOffset + offset
        };
    };
}
