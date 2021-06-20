using Raylib_cs;
using System;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public static class KeyBoard
    {
        public static Action<IKeyBoardEvent> EmitEvent { get; set; }
        private static readonly KeyboardKey[] Keys = Enum.GetValues<KeyboardKey>();

        public static void DoEvents()
        {
            if (IsKeyDown(KeyboardKey.KEY_UP) || IsKeyDown(KeyboardKey.KEY_W))
                EmitEvent(new SnakeUp());

            if (IsKeyDown(KeyboardKey.KEY_RIGHT) || IsKeyDown(KeyboardKey.KEY_D))
                EmitEvent(new SnakeRight());

            if (IsKeyDown(KeyboardKey.KEY_DOWN) || IsKeyDown(KeyboardKey.KEY_S))
                EmitEvent(new SnakeDown());

            if (IsKeyDown(KeyboardKey.KEY_LEFT) || IsKeyDown(KeyboardKey.KEY_A))
                EmitEvent(new SnakeLeft());

        }
    }
}
