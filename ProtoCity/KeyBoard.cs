using Raylib_cs;
using System;
using static Raylib_cs.Raylib;

namespace ProtoCity
{
    public static class KeyBoard
    {
        public static Action<IKeyBoardEvent> EmitEvent { get; set; }
        private static readonly KeyboardKey[] Keys = Enum.GetValues<KeyboardKey>();
        public static void DoEvents()
        {
            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Z))
                EmitEvent(new KeyUndo());

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Y))
                EmitEvent(new KeyRedo());

            if (IsKeyDown(KeyboardKey.KEY_DELETE) || IsKeyDown(KeyboardKey.KEY_D))
                EmitEvent(new KeyDelete());
            
            //bit shit
            foreach(var key in Keys)
            {
                if (IsKeyPressed(key))
                    EmitEvent(new KeyPressed { Char = (char)key });
            }
        }
    }
}
