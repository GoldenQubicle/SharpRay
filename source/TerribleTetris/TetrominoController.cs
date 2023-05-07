namespace TerribleTetris
{
	internal static class TetrominoController
	{
		private static Tetromino _activeTetromino;

		public static void Push(Tetromino tetromino) => _activeTetromino = tetromino;

		public static void OnKeyBoardEvent(IKeyBoardEvent e)
		{

			_activeTetromino.Rotation = e switch
			{
				KeyUpReleased or KeyPressed { KeyboardKey: KeyboardKey.KEY_X } when CanRotateClockwise( ) => RotateClockwise( ),
				KeyPressed { KeyboardKey: KeyboardKey.KEY_LEFT_CONTROL } or
					KeyPressed { KeyboardKey: KeyboardKey.KEY_RIGHT_CONTROL } or
					KeyPressed { KeyboardKey: KeyboardKey.KEY_Z } when CanRotateCounterClockwise( ) => RotateCounterClockwise( ),
				_ => _activeTetromino.Rotation
			};

			_activeTetromino.X = e switch
			{
				KeyRightReleased when CanMoveRight( ) => MoveRight( ),
				KeyLeftReleased when CanMoveLeft( ) => MoveLeft( ),
				_ => _activeTetromino.X
			};
		}

		private static Rotation RotateClockwise() =>
			(Rotation)((int)(_activeTetromino.Rotation + 1) % 4);
		private static Rotation RotateCounterClockwise() =>
			(Rotation)(_activeTetromino.Rotation == 0 ? 3 : (int)_activeTetromino.Rotation - 1);

		private static bool CanRotateClockwise() =>
			CanRotate(RotateClockwise( ));

		private static bool CanRotateCounterClockwise() =>
			CanRotate(RotateCounterClockwise( ));

		private static bool CanRotate(Rotation rotation) =>
			Grid.CanMove(_activeTetromino.GetRotationOffsets(rotation), new Vector2(_activeTetromino.X, _activeTetromino.Y)); 
			// _mapY.end prevented it from rotating on the floor, however, I suspect this will come back to haunt me. 

		private static float MoveLeft() =>
			_activeTetromino.X - Game.GridData.CellSize;

		private static float MoveRight() =>
			_activeTetromino.X + Game.GridData.CellSize;

		private static bool CanMoveLeft() =>
			Grid.CanMove(_activeTetromino.GetLeftMostX( ), new Vector2(MoveLeft( ), _activeTetromino.Y));

		private static bool CanMoveRight() =>
			Grid.CanMove(_activeTetromino.GetRightMostX( ), new Vector2(MoveRight( ), _activeTetromino.Y));
	}
}
