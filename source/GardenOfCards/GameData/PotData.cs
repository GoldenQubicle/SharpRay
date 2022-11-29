namespace GardenOfCards.GameData
{
    internal record PotData(
        int nSlots = 1,
        float BasinHeightFactor = 1.5f,
        int BasinSlant = 30,
        int BasinThickness = 5,
        int RimThickness = 37
        );
}
