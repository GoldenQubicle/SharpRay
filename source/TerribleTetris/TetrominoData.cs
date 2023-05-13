namespace TerribleTetris;

internal static partial class Game
{
	internal static class TetrominoData
	{
		public static int BoundingBoxSize(Shape shape) => shape switch
		{
			Shape.I => 4,
			Shape.O => 2,
			_ => 3
		};

		public static Color Color(Shape shape) => shape switch
		{
			Shape.I => SKYBLUE,
			Shape.O => YELLOW,
			Shape.T => PURPLE,
			Shape.J => DARKBLUE,
			Shape.L => ORANGE,
			Shape.S => LIME,
			Shape.Z => RED,
			Shape.None => BLANK,
			_ => throw new ArgumentOutOfRangeException(nameof(Shape), shape, null)
		};

		private static IDictionary<Shape, Dictionary<Rotation, List<Vector2>>> Offsets =>
			new Dictionary<Shape, Dictionary<Rotation, List<Vector2>>>
			{
				{
					Shape.I, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 1), new(1, 1), new(2, 1), new(3, 1) } },
						{ Rotation.Right, new() { new(2, 0), new(2, 1), new(2, 2), new(2, 3) } },
						{ Rotation.Down, new() { new(0, 2), new(1, 2), new(2, 2), new(3, 2) } },
						{ Rotation.Left, new() { new(1, 0), new(1, 1), new(1, 2), new(1, 3) } },
					}
				},
				{
					Shape.O, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
						{ Rotation.Right, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
						{ Rotation.Down, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
						{ Rotation.Left, new() { new(0, 0), new(0, 1), new(1, 0), new(1, 1) } },
					}
				},
				{
					Shape.T, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(1, 0), new(0, 1), new(1, 1), new(2, 1) } },
						{ Rotation.Right, new() { new(1, 0), new(1, 1), new(2, 1), new(1, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(1, 1), new(2, 1), new(1, 2) } },
						{ Rotation.Left, new() { new(1, 0), new(0, 1), new(1, 1), new(1, 2) } }
					}
				},
				{
					Shape.J, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 0), new(0, 1), new(1, 1), new(2, 1) } },
						{ Rotation.Right, new() { new(1, 0), new(2, 0), new(1, 1), new(1, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(1, 1), new(2, 1), new(2, 2) } },
						{ Rotation.Left, new() { new(0, 2), new(1, 0), new(1, 1), new(1, 2) } },
					}
				},
				{
					Shape.L, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 1), new(1, 1), new(2, 1), new(2, 0) } },
						{ Rotation.Right, new() { new(1, 0), new(1, 1), new(1, 2), new(2, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(0, 2), new(1, 1), new(2, 1) } },
						{ Rotation.Left, new() { new(0, 0), new(1, 0), new(1, 1), new(1, 2) } },
					}
				},
				{
					Shape.S, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(1, 0), new(2, 0), new(0, 1), new(1, 1) } },
						{ Rotation.Right, new() { new(1, 0), new(1, 1), new(2, 1), new(2, 2) } },
						{ Rotation.Down, new() { new(0, 2), new(1, 1), new(1, 2), new(2, 1) } },
						{ Rotation.Left, new() { new(0, 0), new(0, 1), new(1, 1), new(1, 2) } },
					}
				},
				{
					Shape.Z, new Dictionary<Rotation, List<Vector2>>
					{
						{ Rotation.Up, new() { new(0, 0), new(1, 0), new(1, 1), new(2, 1) } },
						{ Rotation.Right, new() { new(2, 0), new(1, 1), new(2, 1), new(1, 2) } },
						{ Rotation.Down, new() { new(0, 1), new(1, 1), new(1, 2), new(2, 2) } },
						{ Rotation.Left, new() { new(0, 1), new(0, 2), new(1, 0), new(1, 1) } },
					}
				},
				{
					Shape.None, new Dictionary<Rotation, List<Vector2>>()
					
				},
			};

		public static List<Vector2> GetOffsets(Shape shape, Rotation rotation) =>
			Offsets[shape][rotation];
	}
}