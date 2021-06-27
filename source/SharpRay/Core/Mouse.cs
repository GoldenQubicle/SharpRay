using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    internal class Mouse
    {
        public static Action<IMouseEvent> EmitEvent { get; set; }

        private static Vector2 PreviousMousePostion;
        public static void DoEvents()
        {
            var currentMousePostion = GetMousePosition();
            var isDragging = currentMousePostion != PreviousMousePostion;
            var mouseWheel = GetMouseWheelMove();

            if (isDragging) 
                EmitEvent?.Invoke(new MouseMovement { Position = currentMousePostion });

            //click events - TODO proper click & double click
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                EmitEvent?.Invoke(new MouseLeftClick { Position = currentMousePostion });

            if (IsMouseButtonPressed(MouseButton.MOUSE_MIDDLE_BUTTON))
                EmitEvent?.Invoke(new MouseMiddleClick { Position = currentMousePostion });

            if (IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                EmitEvent?.Invoke(new MouseRightClick { Position = currentMousePostion });


            //release events
            if (IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                EmitEvent?.Invoke(new MouseLeftRelease { Position = currentMousePostion });

            if (IsMouseButtonReleased(MouseButton.MOUSE_MIDDLE_BUTTON))
                EmitEvent?.Invoke(new MouseMiddleRelease { Position = currentMousePostion });

            if (IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON))
                EmitEvent?.Invoke(new MouseRightRelease { Position = currentMousePostion });


            //drag events
            //send continuously even when cursor is outside window with potentially negative coordinates
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && isDragging)
                EmitEvent?.Invoke(new MouseLeftDrag { Position = currentMousePostion });

            if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON) && isDragging)
                EmitEvent?.Invoke(new MouseMiddleDrag { Position = currentMousePostion });

            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && isDragging)
                EmitEvent?.Invoke(new MouseRightDrag { Position = currentMousePostion });


            //mousewheel events
            if (mouseWheel == 1f)
                EmitEvent?.Invoke(new MouseWheelUp { Position = currentMousePostion });

            if (mouseWheel == -1f)
                EmitEvent?.Invoke(new MouseWheelDown { Position = currentMousePostion });

            PreviousMousePostion = currentMousePostion;
        }
    }
}
