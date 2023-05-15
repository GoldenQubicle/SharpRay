using TerribleTetris.GameMode;

namespace TerribleTetris
{
    internal static class Game
	{
		internal static int WindowHeight => 720;

		internal static int WindowWidth => 1080;

		private static readonly GridData GridData = new(Rows: 16, Cols: 8, CellSize: 35, Color1: RAYWHITE, Color2: LIGHTGRAY);

		internal enum Shape { I, O, T, J, L, S, Z, None };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum Mode { Generation, Playing, Pause }

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double DropTime;

		internal static Stack<Tetromino> TetrominoStack = new( );

		internal static IGameMode GameMode { get; set; } = new PauseMode();

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
				ClearGridAndTetrominos();
				StartGame(Mode.Playing);
			}
		}

		private static void StartGame(Mode mode)
		{
			GameMode = mode switch
			{
				Mode.Generation => GameMode.NextMode(new GenerationMode()),
				Mode.Playing => GameMode.NextMode(new PlayMode()),
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};

			GameMode.OnStart(GridData);
			
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

		private static void SetGridBackgroundTexture(GridData gridData)
		{
			var bgImage = GenImageChecked(gridData.Width, gridData.Height, gridData.CellSize, gridData.CellSize, gridData.Color1, gridData.Color2);
			AddTexture2D("grid", LoadTextureFromImage(bgImage));
			UnloadImage(bgImage);
		}

		internal static bool IsAboveGrid(TetrominoLocked tb) =>
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