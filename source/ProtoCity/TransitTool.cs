using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Gui;
using System.Collections.Generic;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class TransitTool : GuiEntity
    {
        public bool IsActive { get; set; }
        private Dictionary<int, TransitNode> Nodes = new();
        private int prevIdx = -1;
        
        public override void Render()
        {
            var sCenter = GridHandler.GetSelectedCenter();

            foreach (var node in Nodes.Values)
            {
                DrawCircleV(node.Position, 3, Color.DARKPURPLE);
                DrawTextV(node.Connections.Count.ToString(), node.Position, 10, Color.RAYWHITE);

                foreach (var con in node.Connections)
                    DrawLineV(node.Position, con.Position, Color.PURPLE);
            }

            if (prevIdx != -1)
            {
                DrawCircleV(sCenter, 3, Color.PINK);
                DrawLineV(Nodes[prevIdx].Position, sCenter, Color.LIME);
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (!IsActive ) return;

            var (idx, o, c) = GridHandler.GetSelected();

            if (o is not Occupant.None && o is not Occupant.TransitNode) return;

            //TODO handle intersections between segments
            //probably want to be able to insert nodes on existing segments at some point in the future?
            if (e is MouseLeftClick mlc && !mlc.IsHandled)
            {
                if (o is Occupant.None)
                {
                    GridHandler.AddOccupant(idx, Occupant.TransitNode);
                    Nodes.Add(idx, new TransitNode { Idx = idx, Position = c });
                }

                if (prevIdx != -1)
                {
                    //hook up nodes
                    Nodes[idx].Connections.Add(Nodes[prevIdx]);
                    Nodes[prevIdx].Connections.Add(Nodes[idx]);
                }

                prevIdx = idx;
            }

            if (e is MouseRightClick mrc && prevIdx != -1)
            {
                if(Nodes[prevIdx].Connections.Count == 0)
                {
                    Nodes.Remove(prevIdx);
                    GridHandler.RemoveCell(prevIdx);
                }

                prevIdx = -1;
            }
        }
    }
}
