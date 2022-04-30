using SharpRay.Entities;
using SharpRay.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class GuiEvents
    {
        public struct ChangeShipType : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
            public int ShipType { get; init; }
        }

        public struct ChangeShipColor : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
            public string ShipColor { get; init; }
        }

        public struct GameStart : IGuiEvent
        {
            public GuiEntity GuiEntity { get; init; }
        }
    }
}
