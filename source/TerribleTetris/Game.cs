using System.Text.Json;

namespace TerribleTetris
{
	internal static partial class Game
	{
		internal static int WindowHeight => 480;

		internal static int WindowWidth => 680;

		internal enum Shape { I, O, T, J, L, S, Z };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum RotationSystem { Super }

		internal static void Main(string[ ] args)
		{
			SaveJson( );

			Initialize(new SharpRayConfig
			{
				WindowWidth = WindowWidth,
				WindowHeight = WindowHeight,
				DoEventLogging = false,
				ShowFPS = true,
				BackGroundColor = DARKGRAY
			});

			var gridData = new GridData(Rows: 16, Cols: 10, CellSize: 20, Color1: RAYWHITE, Color2: LIGHTGRAY);
			AddEntity(new Grid(gridData));

			var tetrominoData = new TetrominoData(Shape.T);
			AddEntity(new Tetromino(tetrominoData, gridData));

			Run( );
		}

		private static void SaveJson()
		{
			var data = new Dictionary<RotationSystem, Dictionary<Shape, Dictionary<Rotation, List<Vector2>>>>
			{
				{RotationSystem.Super, new Dictionary<Shape, Dictionary<Rotation, List<Vector2>>>
				{
					{ Shape.T , new Dictionary<Rotation, List<Vector2>>
						{
							{ Rotation.Up , new( ) { new (1, 0), new(0, 1), new(1, 1), new(2, 1) } },
							{ Rotation.Right, new ( ) { new(1,0), new(1,1), new(2,1), new(1,2) } },
							{ Rotation.Down, new ( ) { new(0,1), new(1,1), new(2,1), new(1,2) } },
							{ Rotation.Left , new ( ) { new(1,0), new(0,1), new(1,1), new(1,2) } }
						}
					}
				}}
			};

			var json = JsonSerializer.Serialize(data);
			File.WriteAllText(Path.Combine(AssestsFolder, "test.json"), json);
		}
	}
}