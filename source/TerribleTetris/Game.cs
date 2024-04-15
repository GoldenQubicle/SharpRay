﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace TerribleTetris
{
	internal static partial class Game
	{
		internal static int WindowHeight => 720;

		internal static int WindowWidth => 1080;

		internal enum Shape { I, O, T, J, L, S, Z, None };

		internal enum Rotation { Up, Right, Down, Left }

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double LevelTimer = 750;

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

			var gridData = new GridData(Rows: 16, Cols: 16, CellSize: 35, Color1: RAYWHITE, Color2: LIGHTGRAY);
			AddEntity(new Grid(gridData));

			var tetrominoData = new TetrominoData(Shape.L);
			AddEntity(new Tetromino(tetrominoData, gridData));

			Run( );
		}
	}
}