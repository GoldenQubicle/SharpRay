﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using static TerribleTetris.Game;

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

		internal enum RotationSystem { Super } // note default, for now hardcoded

		internal static double LevelTimer = 750;

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
			
			AddEntity(new Grid(GridData));
			

			Enumerable.Range(0, 10).ToList().ForEach(n =>
			{
				var shape = Enum.GetValues<Shape>()[..7][GetRandomValue(0, 7)];
				TetrominoStack.Push(new Tetromino(shape, 5));
			});

			SpawnTetromino();

			SetKeyBoardEventAction(TetrominoController.OnKeyBoardEvent);

			Run( );
		}

		private static void SpawnTetromino()
		{
			var tetromino = TetrominoStack.Pop();
			TetrominoController.Push(tetromino);
			AddEntity(tetromino, OnGameEvent);
		}

		public static void OnGameEvent(IGameEvent e)
		{
			if (e is TetrominoBlocked tb)
			{
				Grid.BlockCells(tb);
				SpawnTetromino();
			}
		}

		/// <summary>
		/// Given the position of the bounding box, and an absolute offset within it (e.g. [1,2]),
		/// calculate the corresponding grid index.  
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="bbPos"></param>
		/// <returns></returns>
		public static (int x, int y) TetrominoOffsetToGridIndices((int x, int y) offset, Vector2 bbPos)
		{
			// The position in screen pixel coordinates.
			var offsetPosition = bbPos + new Vector2(offset.x * GridData.CellSize, offset.y * GridData.CellSize);
			// The position in absolute pixel coordinates
			var absPosition = offsetPosition - GridData.Position;
			// Divide the absolute pixels by the cell size, also in pixels, to arrive at the grid index. 
			return ((int)absPosition.X / GridData.CellSize, (int)absPosition.Y / GridData.CellSize);
		}
	}
}