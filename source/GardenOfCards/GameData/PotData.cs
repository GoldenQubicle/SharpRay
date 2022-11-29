namespace GardenOfCards.GameData
{
    internal record PotData(
        int nSlots = 2,
        float BasinHeightFactor = 1.75f,
        int BasinSlant = 30,
        int BasinThickness = 5,
        int RimThickness = 50
        );
}
