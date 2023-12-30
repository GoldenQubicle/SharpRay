namespace GardenOfCards.GameData
{
	internal record TurnData ( int Number = 1, int HandSize = 4, int HandsPerTurn = 3 )
	{
		public int HandsDealt { get; set; }

		public int HandsLeft => HandsPerTurn - HandsDealt;
	};
}
