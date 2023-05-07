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
			Shape.None => BLANK,
			_ => throw new ArgumentOutOfRangeException(nameof(Shape), Shape, null)
		};

		public IDictionary<Rotation, List<(int x, int y)>> Offsets = Shape switch
		{
			Shape.I => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (0,1), (1,1), (2,1), (3,1) } },
				{ Rotation.Right , new( ) { (2,0), (2,1), (2,2), (2,3) } },
				{ Rotation.Down , new( ) { (0,2), (1,2), (2,2), (3,2) } },
				{ Rotation.Left , new( ) { (1,0), (1,1), (1,2), (1,3) } },
			},
			Shape.O => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (0,0), (0,1), (1,0), (1,1) } },
				{ Rotation.Right , new( ) { (0, 0), (0, 1), (1, 0), (1, 1) } },
				{ Rotation.Down , new( ) { (0, 0), (0, 1), (1, 0), (1, 1) } },
				{ Rotation.Left , new( ) { (0, 0), (0, 1), (1, 0), (1, 1) } },
			},
			Shape.T => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (1, 0), (0, 1), (1, 1), (2, 1) } },
				{ Rotation.Right, new ( ) { (1,0),(1,1), (2,1), (1,2) } },
				{ Rotation.Down, new ( ) { (0,1), (1,1), (2,1), (1,2) } },
				{ Rotation.Left , new ( ) { (1,0), (0,1), (1,1), (1,2) } }
			},
			Shape.J => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (0,0), (0,1), (1,1), (2,1) } },
				{ Rotation.Right , new( ) { (1,0), (2,0), (1,1), (1,2) } },
				{ Rotation.Down , new( ) { (0,1), (1,1), (2,1), (2,2) } },
				{ Rotation.Left , new( ) { (0,2), (1,0), (1,1), (1,2) } },
			},
			Shape.L => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (0,1), (1,1), (2,1), (2,0) } },
				{ Rotation.Right , new( ) { (1,0), (1,1), (1,2), (2,2) } },
				{ Rotation.Down , new( ) { (0,1), (0,2), (1,1), (2,1) } },
				{ Rotation.Left , new( ) { (0,0), (1,0), (1,1), (1,2) } },
			},
			Shape.S => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (1,0), (2,0), (0,1), (1,1) } },
				{ Rotation.Right , new( ) { (1,0), (1,1), (2,1), (2,2) } },
				{ Rotation.Down , new( ) { (0,2), (1,1), (1,2), (2,1) } },
				{ Rotation.Left , new( ) { (0,0), (0,1), (1,1), (1,2) } },
			},
			Shape.Z => new Dictionary<Rotation, List<(int x, int y)>>
			{
				{ Rotation.Up , new( ) { (0,0), (1,0), (1,1), (2,1) } },
				{ Rotation.Right , new( ) { (2,0), (1,1), (2,1), (1,2) } },
				{ Rotation.Down , new( ) { (0,1), (1,1), (1,2), (2,2) } },
				{ Rotation.Left , new( ) { (0,1), (0,2), (1,0), (1,1) } },
			},
			Shape.None => new Dictionary<Rotation, List<(int x, int y)>>(),
			_ => throw new ArgumentOutOfRangeException(nameof(Shape), Shape, null)
		};
	}
}