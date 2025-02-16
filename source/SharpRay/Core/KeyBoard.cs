namespace SharpRay.Core
{
    internal class KeyBoard : IEventEmitter<IKeyBoardEvent>
    {
        public Action<IKeyBoardEvent> EmitEvent { get; set; }
        private static readonly KeyboardKey[] Keys = Enum.GetValues<KeyboardKey>();

        public void DoEvents()
        {

            foreach (var key in Keys) if (IsKeyPressed(key)) DoEvent(new KeyPressed { KeyboardKey = key });


            if (IsKeyReleased(KeyboardKey.Up) || IsKeyReleased(KeyboardKey.W))
                DoEvent(new KeyUpReleased());

            if (IsKeyReleased(KeyboardKey.Right) || IsKeyReleased(KeyboardKey.D))
                DoEvent(new KeyRightReleased());

            if (IsKeyReleased(KeyboardKey.Down) || IsKeyReleased(KeyboardKey.S))
                DoEvent(new KeyDownReleased());

            if (IsKeyReleased(KeyboardKey.Left) || IsKeyReleased(KeyboardKey.A))
                DoEvent(new KeyLeftReleased());


            if (IsKeyDown(KeyboardKey.Up) || IsKeyDown(KeyboardKey.W))
                DoEvent(new KeyUpDown());

            if (IsKeyDown(KeyboardKey.Right) || IsKeyDown(KeyboardKey.D))
                DoEvent(new KeyRightDown());

            if (IsKeyDown(KeyboardKey.Down) || IsKeyDown(KeyboardKey.S))
                DoEvent(new KeyDownDown());

            if (IsKeyDown(KeyboardKey.Left) || IsKeyDown(KeyboardKey.A))
                DoEvent(new KeyLeftDown());


            if (IsKeyPressed(KeyboardKey.Space))
                DoEvent(new KeySpaceBarPressed());

            if (IsKeyDown(KeyboardKey.Space))
                DoEvent(new KeySpaceBarDown());

            if (IsKeyDown(KeyboardKey.Delete))
                DoEvent(new KeyDelete());


            if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.Z))
                DoEvent(new KeyUndo());

            if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.Y))
                DoEvent(new KeyRedo());
        }

        private void DoEvent(IKeyBoardEvent kbe) => EmitEvent?.Invoke(kbe);
    }
}
