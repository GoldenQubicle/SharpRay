using TerribleTetris.GameMode;

namespace TerribleTetris
{
	internal static class Game
	{
		internal static int WindowHeight => 720;

		internal static int WindowWidth => 1080;
		internal static int CellSize => 25;
		internal static GridData GridData = new(Rows: 14, Cols: 8, CellSize: CellSize);
		internal const string GridTexture = nameof(GridTexture);
		internal enum Shape { I, O, T, J, L, S, Z, None };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum Mode { Generation, Playing, Pause }

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double DropTime;

		internal static Stack<Tetromino> TetrominoStack = new( );

		internal static IGameMode GameMode { get; set; } = new PauseMode { GridData = GridData };

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

			SetGridBackgroundTexture(GridData);

			StartGame(Mode.Generation);

			Run( );
		}

		private static void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (e is KeyPressed { KeyboardKey: KeyboardKey.KEY_SPACE })
			{
				ClearGridAndTetrominos( );
				StartGame(Mode.Generation);
			}

			if (e is KeyPressed { KeyboardKey: KeyboardKey.KEY_P } && GameMode is PauseMode)
			{
				ClearGridAndTetrominos( );
				GameMode = GameMode.NextMode(new PlayMode( ));
				GameMode.Initialize( );
			}
		}

		private static void StartGame(Mode mode, string fileName = "")
		{
			GameMode = mode switch
			{
				Mode.Generation => new GenerationMode(GridData, fileName),
				Mode.Playing => new PlayMode(fileName),
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};

			GameMode.Initialize( );

		}

		private static void ClearGridAndTetrominos()
		{
			RemoveEntitiesOfType<Grid>( );
			RemoveEntitiesOfType<Tetromino>( );
		}

		internal static void SpawnTetromino()
		{
			if (!TetrominoStack.Any( ))
				return;

			AddEntity(TetrominoStack.Pop( ), GameMode.OnGameEvent);
		}

		internal static void SetGridBackgroundTexture(GridData gridData)
		{
			RemoveTexture2D(GridTexture);
			var bgImage = GenImageChecked(gridData.Width, gridData.Height, gridData.CellSize, gridData.CellSize, RAYWHITE, LIGHTGRAY);
			AddTexture2D(GridTexture, LoadTextureFromImage(bgImage));
			UnloadImage(bgImage);
		}

		internal static bool IsAboveGrid(TetrominoLocked tb) =>
			Tetromino.GetOffsets(tb.Shape, tb.Rotation)
				.Any(o => OffsetToGridIdx(o, tb.BbIndex).Y <= 0);

		internal static Vector2 OffsetToGridIdx(Vector2 bbIdx, Vector2 offset) => bbIdx + offset;

		internal static Vector2 OffsetToScreen(Vector2 bbIdx, Vector2 offset) =>
			 BbIdxToScreen(bbIdx) + ( offset * CellSize );

		internal static Vector2 BbIdxToScreen(Vector2 bbIdx) =>
			GridData.Position + ( bbIdx * CellSize );

		internal static JsonSerializerOptions GetJsonOptions() => new( )
		{
			WriteIndented = true,
			Converters =
			{
				new Vector2JsonConverter()
			}
		};

	}
}