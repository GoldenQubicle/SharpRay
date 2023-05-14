using static TerribleTetris.Game;

namespace TerribleTetris
{
	internal static partial class Game
	{
		internal static int WindowHeight => 720;

		internal static int WindowWidth => 1080;

		internal static GridData GridData =
			new(Rows: 16, Cols: 8, CellSize: 35, Color1: RAYWHITE, Color2: LIGHTGRAY);

		internal enum Shape { I, O, T, J, L, S, Z, None };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum Mode { Generation, Playing, Pause }

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double DropTime;

		internal static Stack<Tetromino> TetrominoStack = new( );
		internal static PatternData? Pattern;

		internal static Mode CurrentMode { get; set; }

		internal static void Main(string[ ] args)
		{
			Initialize(new SharpRayConfig
			{
				WindowWidth = WindowWidth,
				WindowHeight = WindowHeight,
				DoEventLogging = false,
				ShowFPS = true,
				BackGroundColor = DARKGRAY
			});


			SetKeyBoardEventAction(OnKeyBoardEvent);

			SetGridBackgroundTexture( );

			StartGame(Mode.Generation);

			Run( );
		}

		public static void OnGameEvent(IGameEvent e)
		{
			if (e is TetrominoLocked tb)
			{
				GetEntity<Grid>( ).LockCells(tb);

				if (CurrentMode == Mode.Generation)
				{
					Pattern.Shapes.Add(tb);

					if (IsAboveGrid(tb))
					{

						CurrentMode = Mode.Pause;
						return;
					}
				}

				if (CurrentMode == Mode.Playing)
				{
					Pattern.Placed.Add(tb);

					if (IsAboveGrid(tb) || TetrominoStack.Count == 0)
					{
						Print($"Game Over!");
						return;
					}
				}

				SpawnTetromino( );
			}
		}

		private static void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (e is KeyPressed { KeyboardKey: KeyboardKey.KEY_SPACE })
			{
				ClearGridAndTetrominos( );
				StartGame(Mode.Generation);
			}

			if (e is KeyPressed { KeyboardKey: KeyboardKey.KEY_P } && CurrentMode == Mode.Pause)
			{
				ClearGridAndTetrominos();
				StartGame(Mode.Playing);
			}
		}

		private static void StartGame(Mode mode)
		{
			CurrentMode = mode;
			AddEntity(new Grid(GridData));

			switch (CurrentMode)
			{
				case Mode.Generation:
					DropTime = 10;
					TetrominoStack.Clear( );
					Pattern = new PatternData(GridData.Rows, GridData.Cols, new( ));
					var shapes = Enum.GetValues<Shape>( )[..7].ToList( );
					var rotations = Enum.GetValues<Rotation>( );
					var bagCount = 0;
					while (bagCount < 5)
					{
						var shape = shapes[GetRandomValue(0, shapes.Count - 1)];
						shapes.Remove(shape);
						TetrominoStack.Push(new Tetromino(shape, rotations[GetRandomValue(0, 3)], GetRandomValue(0, GridData.Cols - Tetromino.BoundingBoxSize(shape))));

						if (shapes.Count != 0)
							continue;

						shapes = Enum.GetValues<Shape>( )[..7].ToList( );
						bagCount++;
					}
					SpawnTetromino( );
					break;

				case Mode.Playing:
					DropTime = 750;
					TetrominoStack.Clear();
					Pattern.Shapes.Reverse( );
					Pattern.Shapes.ForEach(s => TetrominoStack.Push(new Tetromino(s.Shape, Rotation.Up, ( GridData.Cols - Tetromino.BoundingBoxSize(s.Shape))/2)));
					AddEntity(new Pattern(Pattern));
					SpawnTetromino( );
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		private static void ClearGridAndTetrominos()
		{
			RemoveEntitiesOfType<Grid>( );
			RemoveEntitiesOfType<Tetromino>( );
		}
		
		private static void SpawnTetromino()
		{
			if (!TetrominoStack.Any( ))
				return;

			AddEntity(TetrominoStack.Pop( ), OnGameEvent);
		}

		private static void SetGridBackgroundTexture()
		{
			var bgImage = GenImageChecked(GridData.Width, GridData.Height, GridData.CellSize, GridData.CellSize, GridData.Color1, GridData.Color2);
			AddTexture2D("grid", LoadTextureFromImage(bgImage));
			UnloadImage(bgImage);
		}

		private static bool IsAboveGrid(TetrominoLocked tb) =>
			Tetromino.GetOffsets(tb.Shape, tb.Rotation)
				.Any(o => OffsetToGridIdx(o, tb.BbIndex).Y <= 0);

		internal static Vector2 OffsetToGridIdx(Vector2 bbIdx, Vector2 offset) => bbIdx + offset;

		internal static Vector2 OffsetToScreen(Vector2 bbIdx, Vector2 offset) =>
			 BbIdxToScreen(bbIdx) + ( offset * GridData.CellSize );

		internal static Vector2 BbIdxToScreen(Vector2 bbIdx) =>
			GridData.Position + ( bbIdx * GridData.CellSize );

		private static JsonSerializerOptions GetJsonOptions() => new( )
		{
			WriteIndented = true,
			Converters =
			{
				new Vector2JsonConverter()
			}
		};

	}
}