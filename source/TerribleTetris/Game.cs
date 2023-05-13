namespace TerribleTetris
{
	internal static partial class Game
	{
		internal static int WindowHeight => 720;

		internal static int WindowWidth => 1080;

		internal static GridData GridData =
			new(Rows: 16, Cols: 16, CellSize: 35, Color1: RAYWHITE, Color2: LIGHTGRAY);

		internal enum Shape { I, O, T, J, L, S, Z, None };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum Mode { Generation, Playing }

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double DropTime;

		internal static Stack<Tetromino> TetrominoStack = new( );
		internal static PatternData _pattern;

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

			StartGame(Mode.Playing);

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
			CurrentMode = mode;
			AddEntity(new Grid(GridData));
			
			switch (CurrentMode)
			{
				case Mode.Generation:
					DropTime = 100;
					TetrominoStack.Clear( );
					_pattern = new PatternData(GridData.Rows, GridData.Cols, new( ));
					var shapes = Enum.GetValues<Shape>( )[..7].ToList( );
					var bagCount = 0;
					while (bagCount < 5)
					{
						var shape = shapes[GetRandomValue(0, shapes.Count - 1)];
						shapes.Remove(shape);
						TetrominoStack.Push(new Tetromino(shape, GetRandomValue(0, GridData.Cols - TetrominoData.BoundingBoxSize(shape))));

						if (shapes.Count != 0)
							continue;

						shapes = Enum.GetValues<Shape>( )[..7].ToList( );
						bagCount++;
					}
					SpawnTetromino( );
					break;

				case Mode.Playing:
					DropTime = 750;
					var json = File.ReadAllText(Path.Combine(AssestsFolder, "readtest.json"));
					var pattern = JsonSerializer.Deserialize<PatternData>(json, GetJsonOptions( ));
					GridData = GridData with { Cols = pattern.Cols, Rows = pattern.Rows };
					pattern.Shapes.Reverse();
					pattern.Shapes.ForEach(s => TetrominoStack.Push(new Tetromino(s.Shape, 5)));
					AddEntity(new Pattern(pattern));
					SpawnTetromino( );

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		private static void OnKeyBoardEvent(IKeyBoardEvent e)
		{
			if (e is KeyPressed { KeyboardKey: KeyboardKey.KEY_SPACE })
			{
				RemoveEntitiesOfType<Grid>( );
				RemoveEntitiesOfType<Tetromino>( );
				StartGame(Mode.Generation);
			}
		}

		private static void SpawnTetromino()
		{
			if (!TetrominoStack.Any( ))
				return;

			AddEntity(TetrominoStack.Pop( ), OnGameEvent);
		}

		public static void OnGameEvent(IGameEvent e)
		{
			if (e is TetrominoLocked tb)
			{
				GetEntity<Grid>( ).LockCells(tb);

				if (CurrentMode == Mode.Generation)
				{
					_pattern.Shapes.Add(tb);

					if (IsAboveGrid(tb))
					{
						var json = JsonSerializer.Serialize(_pattern, GetJsonOptions());
						File.WriteAllText(Path.Combine(AssestsFolder, "readtest.json"), json);
						return;
					}
				}

				if (CurrentMode == Mode.Playing && (IsAboveGrid(tb) || TetrominoStack.Count == 0))
				{
					//TODO calculate the score
					Print("Game Over");
					return;
				}
				
				SpawnTetromino( );
			}
		}


		private static bool IsAboveGrid(TetrominoLocked tb) =>
			TetrominoData.GetOffsets(tb.Shape, tb.Rotation)
				.Any(o => TetrominoOffsetToGridIndices(o, tb.BbIndex).Y <= 0);

		internal static Vector2 TetrominoOffsetToGridIndices(Vector2 offset, Vector2 bbPos) => offset + bbPos;

		internal static Vector2 OffsetToScreen(Vector2 index, Vector2 offset) => 
			 IndexToScreen(index) + new Vector2(offset.X * GridData.CellSize, offset.Y * GridData.CellSize);

		internal static Vector2 IndexToScreen(Vector2 index) =>
			GridData.Position + ( index * GridData.CellSize );

		private static JsonSerializerOptions GetJsonOptions() => new()
			{
				WriteIndented = true, 
				Converters =
					{
						new Vector2JsonConverter()
					}
			};

	}
}