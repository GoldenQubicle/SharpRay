namespace GardenOfCards.GameData
{
    internal record PotData(
        int nSlots = 3,
        float BasinHeightFactor = 1.5f,
        int BasinSlant = 30,
        int BasinThickness = 6,
        int RimThickness = 34
        );
}
