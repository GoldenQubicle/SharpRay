using Raylib_cs;
using System;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public static class KeyBoard
    {
        public static Action<IKeyBoardEvent> EmitKeyBoardEvent { get; set; }
        private static readonly KeyboardKey[] Keys = Enum.GetValues<KeyboardKey>();

        public static void DoEvents()
        {
            if (IsKeyDown(KeyboardKey.KEY_UP) || IsKeyDown(KeyboardKey.KEY_W))
                EmitKeyBoardEvent(new KeyUp());

            if (IsKeyDown(KeyboardKey.KEY_RIGHT) || IsKeyDown(KeyboardKey.KEY_D))
                EmitKeyBoardEvent(new KeyRight());

            if (IsKeyDown(KeyboardKey.KEY_DOWN) || IsKeyDown(KeyboardKey.KEY_S))
                EmitKeyBoardEvent(new KeyDown());

            if (IsKeyDown(KeyboardKey.KEY_LEFT) || IsKeyDown(KeyboardKey.KEY_A))
                EmitKeyBoardEvent(new KeyLeft());


            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Z))
                EmitKeyBoardEvent(new KeyUndo());

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Y))
                EmitKeyBoardEvent(new KeyRedo());


            if (IsKeyDown(KeyboardKey.KEY_DELETE))
                EmitKeyBoardEvent(new KeyDelete());

            //bit shit
            foreach (var key in Keys)
            {
                if (IsKeyPressed(key)) EmitKeyBoardEvent(new KeyPressed { Char = (char)key });
            }
        }
    }
}
