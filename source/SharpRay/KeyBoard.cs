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
                EmitEvent(new KeyUp());

            if (IsKeyDown(KeyboardKey.KEY_RIGHT) || IsKeyDown(KeyboardKey.KEY_D))
                EmitEvent(new KeyRight());

            if (IsKeyDown(KeyboardKey.KEY_DOWN) || IsKeyDown(KeyboardKey.KEY_S))
                EmitEvent(new KeyDown());

            if (IsKeyDown(KeyboardKey.KEY_LEFT) || IsKeyDown(KeyboardKey.KEY_A))
                EmitEvent(new KeyLeft());


            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Z))
                EmitEvent(new KeyUndo());

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Y))
                EmitEvent(new KeyRedo());


            if (IsKeyDown(KeyboardKey.KEY_DELETE))
                EmitEvent(new KeyDelete());

            //bit shit
            foreach (var key in Keys)
            {
                if (IsKeyPressed(key)) EmitEvent(new KeyPressed { Char = (char)key });
            }
        }
    }
}
