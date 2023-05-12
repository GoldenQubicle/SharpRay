namespace TerribleTetris
{
	internal static partial class Game
	{
		internal static int WindowHeight => 720;

		internal static int WindowWidth => 1080;

		internal static readonly GridData GridData = 
			new(Rows: 16, Cols: 16, CellSize: 35, Color1: RAYWHITE, Color2: LIGHTGRAY);

		internal enum Shape { I, O, T, J, L, S, Z, None };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum Mode { Generation, Playing }

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double DropTime = 10;

		internal static Stack<Tetromino> TetrominoStack = new();

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

			SetGridBackgroundTexture();

			StartGame(Mode.Generation);

			Run( );
		}

		private static void SetGridBackgroundTexture()
		{
			var bgImage = GenImageChecked(GridData.Width, GridData.Height, GridData.CellSize, GridData.CellSize, GridData.Color1, GridData.Color2);
			AddTexture2D("grid", LoadTextureFromImage(bgImage));
			UnloadImage(bgImage);
		}

		private static void StartGame(Mode mode)
		{
			AddEntity(new Grid(GridData));

			switch (mode)
			{
				case Mode.Generation:
					TetrominoStack.Clear( );
					var shapes = Enum.GetValues<Shape>( )[..7].ToList( );
					var bagCount = 0;
					while (bagCount < 5)
					{
						var shape = shapes[GetRandomValue(0, shapes.Count - 1)];
						shapes.Remove(shape);
						var data = new TetrominoData(shape);
						TetrominoStack.Push(new Tetromino(data, GetRandomValue(0, GridData.Cols - data.BoundingBoxSize)));

						if (shapes.Count != 0)
							continue;

						shapes = Enum.GetValues<Shape>( )[..7].ToList( );
						bagCount++;
					}
					SpawnTetromino( );
					break;
				case Mode.Playing:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		private static void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (e is KeyPressed { KeyboardKey: KeyboardKey.KEY_SPACE })
			{
				RemoveEntitiesOfType<Grid>();
				RemoveEntitiesOfType<Tetromino>();
				StartGame(Mode.Generation);
			}
		}

		private static void SpawnTetromino()
		{
			if (!TetrominoStack.Any()) return;

			AddEntity(TetrominoStack.Pop( ), OnGameEvent);
		}

		public static void OnGameEvent(IGameEvent e)
		{
			if (e is TetrominoLocked tb)
			{
				GetEntity<Grid>( ).LockCells(tb);

				if (tb.Offsets.Any(o => TetrominoOffsetToGridIndices(o, tb.BbIndex).y <= 0))
				{
					Print("Game OVer");
				}
				else
				{
					SpawnTetromino( );
				}

				
			}
		}

		/// <summary>
		/// Given the indices of the bounding box, and an absolute offset within it (e.g. [1,2]),
		/// calculate the corresponding grid index of the offset.  
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="bbPos"></param>
		/// <returns></returns>
		public static (int x, int y) TetrominoOffsetToGridIndices((int x, int y) offset, Vector2 bbPos) => 
			(offset.x + (int)bbPos.X, offset.y + (int)bbPos.Y);

	}
}