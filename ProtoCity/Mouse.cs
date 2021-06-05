using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace ProtoCity
{
    public static class Mouse
    {
        public static List<Action<IMouseEvent>> Actions { get; } = new();

        private static Vector2 PreviousMousePostion;
        public static void DoEvents()
        {
            var currentMousePostion = GetMousePosition();
            var isDragging = currentMousePostion != PreviousMousePostion;
            var mouseWheel = GetMouseWheelMove();

            //click events
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                Actions.ForEach(a => a(new MouseLeftClick { Position = currentMousePostion }));

            if (IsMouseButtonPressed(MouseButton.MOUSE_MIDDLE_BUTTON))
                Actions.ForEach(a => a(new MouseMiddleClick { Position = currentMousePostion }));

            if (IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                Actions.ForEach(a => a(new MouseRighttClick { Position = currentMousePostion }));


            //release events
            if (IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                Actions.ForEach(a => a(new MouseLeftRelease { Position = currentMousePostion }));

            if (IsMouseButtonReleased(MouseButton.MOUSE_MIDDLE_BUTTON))
                Actions.ForEach(a => a(new MouseMiddleRelease { Position = currentMousePostion }));

            if (IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON))
                Actions.ForEach(a => a(new MouseRightRelease { Position = currentMousePostion }));


            //drag events
            //send continuously even when cursor is outside window with potentially negative coordinates
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && isDragging)
                Actions.ForEach(a => a(new MouseLeftDrag { Position = currentMousePostion }));

            if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON) && isDragging)
                Actions.ForEach(a => a(new MouseMiddleDrag { Position = currentMousePostion }));

            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && isDragging)
                Actions.ForEach(a => a(new MouseRightDrag { Position = currentMousePostion }));


            //mousewheel events
            if (mouseWheel == 1f)
                Actions.ForEach(a => a(new MouseWheelUp { Position = currentMousePostion }));

            if (mouseWheel == -1f)
                Actions.ForEach(a => a(new MouseWheelDown { Position = currentMousePostion }));

            PreviousMousePostion = currentMousePostion;
        }
    }
}
