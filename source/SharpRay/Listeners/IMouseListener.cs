﻿using SharpRay.Eventing;

namespace SharpRay.Listeners
{
    public interface IMouseListener
    {
        void OnMouseEvent(IMouseEvent e);
    }
}
