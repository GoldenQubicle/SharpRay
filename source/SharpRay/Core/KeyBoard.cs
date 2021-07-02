using Raylib_cs;
using System;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    internal static class KeyBoard
    {
        public static Action<IKeyBoardEvent> EmitEvent { get; set; }
        private static readonly KeyboardKey[] Keys = Enum.GetValues<KeyboardKey>();

        public static void DoEvents()
        {
            if (IsKeyDown(KeyboardKey.KEY_UP) || IsKeyDown(KeyboardKey.KEY_W))
                EmitEvent?.Invoke(new KeyUp());

            if (IsKeyDown(KeyboardKey.KEY_RIGHT) || IsKeyDown(KeyboardKey.KEY_D))
                EmitEvent?.Invoke(new KeyRight());

            if (IsKeyDown(KeyboardKey.KEY_DOWN) || IsKeyDown(KeyboardKey.KEY_S))
                EmitEvent?.Invoke(new KeyDown());

            if (IsKeyDown(KeyboardKey.KEY_LEFT) || IsKeyDown(KeyboardKey.KEY_A))
                EmitEvent?.Invoke(new KeyLeft());

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Z))
                EmitEvent?.Invoke(new KeyUndo());

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Y))
                EmitEvent?.Invoke(new KeyRedo());

            if (IsKeyDown(KeyboardKey.KEY_DELETE))
                EmitEvent?.Invoke(new KeyDelete());
        }
    }
}
