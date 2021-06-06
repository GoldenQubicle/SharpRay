﻿using Raylib_cs;
using System;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace SharpRay
{
    public class Mouse
    {
        public static Action<IMouseEvent> EmitEvent { get; set; }

        private static Vector2 PreviousMousePostion;
        public static void DoEvents()
        {
            var currentMousePostion = GetMousePosition();
            var isDragging = currentMousePostion != PreviousMousePostion;
            var mouseWheel = GetMouseWheelMove();

            //click events - TODO proper click & double click
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                EmitEvent(new MouseLeftClick { Position = currentMousePostion });

            if (IsMouseButtonPressed(MouseButton.MOUSE_MIDDLE_BUTTON))
                EmitEvent(new MouseMiddleClick { Position = currentMousePostion });

            if (IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                EmitEvent(new MouseRightClick { Position = currentMousePostion });


            //release events
            if (IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                EmitEvent(new MouseLeftRelease { Position = currentMousePostion });

            if (IsMouseButtonReleased(MouseButton.MOUSE_MIDDLE_BUTTON))
                EmitEvent(new MouseMiddleRelease { Position = currentMousePostion });

            if (IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON))
                EmitEvent(new MouseRightRelease { Position = currentMousePostion });


            //drag events
            //send continuously even when cursor is outside window with potentially negative coordinates
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && isDragging)
                EmitEvent(new MouseLeftDrag { Position = currentMousePostion });

            if (IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON) && isDragging)
                EmitEvent(new MouseMiddleDrag { Position = currentMousePostion });

            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON) && isDragging)
                EmitEvent(new MouseRightDrag { Position = currentMousePostion });


            //mousewheel events
            if (mouseWheel == 1f)
                EmitEvent(new MouseWheelUp { Position = currentMousePostion });

            if (mouseWheel == -1f)
                EmitEvent(new MouseWheelDown { Position = currentMousePostion });

            PreviousMousePostion = currentMousePostion;
        }
    }
}
