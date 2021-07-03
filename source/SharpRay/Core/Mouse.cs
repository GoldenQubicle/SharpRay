using Raylib_cs;
using System;
using System.Numerics;
using SharpRay.Eventing;
using static Raylib_cs.Raylib;
using SharpRay.Listeners;

namespace SharpRay.Core
{
    internal class Mouse : IEventEmitter<IMouseEvent>
    {
        public Action<IMouseEvent> EmitEvent { get; set; }

        private static Vector2 PreviousMousePostion;
        public static Vector2 Cursor { get; private set; }

        public void DoEvents()
        {
            Cursor = GetMousePosition();

            var isDragging = Cursor != PreviousMousePostion;
            var mouseWheel = GetMouseWheelMove();


            if (isDragging) 
                EmitEvent?.Invoke(new MouseMovement { Position = Cursor });

            //click events - TODO proper click & double click
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                EmitEvent?.Invoke(new MouseLeftClick { Position = Cursor });

            if (IsMouseButtonPressed(MouseButton.MOUSE_MIDDLE_BUTTON))
                EmitEvent?.Invoke(new MouseMiddleClick { Position = Cursor });

            if (IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                EmitEvent?.Invoke(new MouseRightClick { Position = Cursor });


            //release events
            if (IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                EmitEvent?.Invoke(new MouseLeftRelease { Position = Cursor });

            if (IsMouseButtonReleased(MouseButton.MOUSE_MIDDLE_BUTTON))
                EmitEvent?.Invoke(new MouseMiddleRelease { Position = Cursor });

            if (IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON))
                EmitEvent?.Invoke(new MouseRightRelease { Position = Cursor });


            //drag events
            //send continuously even when cursor is outside window with potentially negative coordinates
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && isDragging)
                EmitEvent?.Invoke(new MouseLeftDrag { Position = Cursor });

            if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON) && isDragging)
                EmitEvent?.Invoke(new MouseMiddleDrag { Position = Cursor });

            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && isDragging)
                EmitEvent?.Invoke(new MouseRightDrag { Position = Cursor });


            //mousewheel events
            if (mouseWheel == 1f)
                EmitEvent?.Invoke(new MouseWheelUp { Position = Cursor });

            if (mouseWheel == -1f)
                EmitEvent?.Invoke(new MouseWheelDown { Position = Cursor });

            PreviousMousePostion = Cursor;
        }
    }
}
