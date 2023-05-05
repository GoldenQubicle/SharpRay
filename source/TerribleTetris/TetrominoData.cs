namespace TerribleTetris;

internal static partial class Game
{
	internal record TetrominoData(Shape Shape)
	{
		public int BoundingBoxSize = Shape switch
		{
			Shape.I => 4,
			Shape.O => 2,
			_ => 3
		};

		public Color Color = Shape switch
		{
			Shape.I => SKYBLUE,
			Shape.O => YELLOW,
			Shape.T => PURPLE,
			Shape.J => DARKBLUE,
			Shape.L => ORANGE,
			Shape.S => LIME,
			Shape.Z => RED,
			_ => throw new ArgumentOutOfRangeException(nameof(Shape), Shape, null)
		};

		public IDictionary<Rotation, List<(int x, int y)>> Offsets = Shape switch
		{
			//Shape.I => new( ) { (0, 0), (1, 0), (2, 0), (3, 0) },
			//Shape.O => new( ) { (0, 0), (1, 0), (0, 1), (1, 1) },
			Shape.T => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (1, 0), (0, 1), (1, 1), (2, 1) } },
				{ Rotation.Right, new (){ (1,0),(1,1), (2,1), (1,2) } },
				{ Rotation.Down, new (){ (0,1), (1,1), (2,1), (1,2) } },
				{ Rotation.Left , new () { (1,0), (0,1), (1,1), (1,2) } }
			},
			//Shape.J => new( ) { (1, 0), (1, 1), (1, 2), (0, 2) },
			//Shape.L => new( ) { (1, 0), (1, 1), (1, 2), (2, 2) },
			//Shape.S => new( ) { (1, 0), (2, 0), (0, 1), (1, 1) },
			//Shape.Z => new( ) { (0, 0), (1, 0), (1, 1), (2, 1) },
			_ => throw new ArgumentOutOfRangeException(nameof(Shape), Shape, null)
		};
	}
}