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
                Actions.ForEach(a => a(new MouseLeftClick(currentMousePostion)));

            if (IsMouseButtonPressed(MouseButton.MOUSE_MIDDLE_BUTTON))
                Actions.ForEach(a => a(new MouseMiddleClick(currentMousePostion)));

            if (IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                Actions.ForEach(a => a(new MouseRighttClick(currentMousePostion)));


            //release events
            if (IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                Actions.ForEach(a => a(new MouseLeftRelease(currentMousePostion)));

            if (IsMouseButtonReleased(MouseButton.MOUSE_MIDDLE_BUTTON))
                Actions.ForEach(a => a(new MouseMiddleRelease(currentMousePostion)));

            if (IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON))
                Actions.ForEach(a => a(new MouseRightRelease(currentMousePostion)));


            //drag events
            //send continuously even when cursor is outside window with potentially negative coordinates
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && isDragging)
                Actions.ForEach(a => a(new MouseLeftDrag(currentMousePostion)));

            if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON) && isDragging)
                Actions.ForEach(a => a(new MouseMiddleDrag(currentMousePostion)));

            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && isDragging)
                Actions.ForEach(a => a(new MouseRightDrag(currentMousePostion)));


            //mousewheel events
            if (mouseWheel == 1f)
                Actions.ForEach(a => a(new MouseWheelUp(currentMousePostion)));

            if (mouseWheel == -1f)
                Actions.ForEach(a => a(new MouseWheelDown(currentMousePostion)));

            PreviousMousePostion = currentMousePostion;
        }

        public static Vector2 GetPosition() => GetMousePosition();
    }
}
