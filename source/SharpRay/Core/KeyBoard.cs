using Raylib_cs;
using System;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;

namespace SharpRay.Core
{
    internal static class KeyBoard
    {
        public static Action<IKeyBoardEvent> EmitEvent { get; set; }
        private static readonly KeyboardKey[] Keys = Enum.GetValues<KeyboardKey>();

        public static void DoEvents()
        {

            foreach (var key in Keys) if (IsKeyPressed(key)) DoEvent(new KeyPressed { KeyboardKey = key });


            if (IsKeyReleased(KeyboardKey.KEY_UP) || IsKeyReleased(KeyboardKey.KEY_W))
                DoEvent(new KeyUpReleased());

            if (IsKeyReleased(KeyboardKey.KEY_RIGHT) || IsKeyReleased(KeyboardKey.KEY_D))
                DoEvent(new KeyRightReleased());

            if (IsKeyReleased(KeyboardKey.KEY_DOWN) || IsKeyReleased(KeyboardKey.KEY_S))
                DoEvent(new KeyDownReleased());

            if (IsKeyReleased(KeyboardKey.KEY_LEFT) || IsKeyReleased(KeyboardKey.KEY_A))
                DoEvent(new KeyLeftReleased());


            if (IsKeyDown(KeyboardKey.KEY_UP) || IsKeyDown(KeyboardKey.KEY_W))
                DoEvent(new KeyUpDown());

            if (IsKeyDown(KeyboardKey.KEY_RIGHT) || IsKeyDown(KeyboardKey.KEY_D))
                DoEvent(new KeyRightDown());

            if (IsKeyDown(KeyboardKey.KEY_DOWN) || IsKeyDown(KeyboardKey.KEY_S))
                DoEvent(new KeyDownDown());

            if (IsKeyDown(KeyboardKey.KEY_LEFT) || IsKeyDown(KeyboardKey.KEY_A))
                DoEvent(new KeyLeftDown());


            if (IsKeyPressed(KeyboardKey.KEY_SPACE))
                DoEvent(new KeySpaceBarPressed());

            if (IsKeyDown(KeyboardKey.KEY_DELETE))
                DoEvent(new KeyDelete());


            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Z))
                DoEvent(new KeyUndo());

            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_Y))
                DoEvent(new KeyRedo());
        }

        private static void DoEvent(IKeyBoardEvent kbe) => EmitEvent?.Invoke(kbe);
    }
}
