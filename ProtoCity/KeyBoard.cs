using Raylib_cs;
using System;
using System.Collections.Generic;
using static Raylib_cs.Raylib;

namespace ProtoCity
{
    public static class KeyBoard
    {
        public static List<Action<IKeyBoardEvent>> Actions { get; } = new();

        public static void DoEvents()
        {

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Z))
                Actions.ForEach(a => a(new KeyUndo()));

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Y))
                Actions.ForEach(a => a(new KeyRedo()));

            if (IsKeyDown(KeyboardKey.KEY_DELETE) || IsKeyDown(KeyboardKey.KEY_D))
                Actions.ForEach(a => a(new KeyDelete()));
        }
    }
}
