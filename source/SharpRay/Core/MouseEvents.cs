using System.Numerics;
using SharpRay.Eventing;

namespace SharpRay.Core
{

    public class MouseLeftClick : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }
    }

    public class MouseMiddleClick : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseRightClick : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseLeftDrag : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseMiddleDrag : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseRightDrag : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseLeftRelease : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseMiddleRelease : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseRightRelease : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseWheelUp : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseWheelDown : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }

    public class MouseMovement : IMouseEvent
    {
        public Vector2 Position { get; init; }
        public bool IsHandled { get; set; }

    }
}
