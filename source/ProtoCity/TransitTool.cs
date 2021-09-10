using Raylib_cs;
using SharpRay.Core;
using SharpRay.Entities;
using SharpRay.Eventing;
using SharpRay.Gui;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public class TransitTool : Entity
    {
        private bool isActive = true;
        private Dictionary<int, TransitNode> Nodes = new();
        private int prevIdx = -1;
        public override void Render()
        {
            DrawText($"Transit Tool :  {(isActive ? "Active" : "InActive")}", 100, 10, 15, Color.RAYWHITE);
            foreach (var node in Nodes.Values)
            {
                DrawCircleV(node.Position, 3, Color.DARKPURPLE);
                DrawTextV(node.Connections.Count.ToString(), node.Position, 10, Color.RAYWHITE);
               
                foreach (var con in node.Connections)
                    DrawLineV(node.Position, con.Position, Color.PURPLE);
            }

            if (prevIdx != -1)
            {
                DrawCircleV(GridHandler.SelectedCellCenter, 3, Color.PINK);
                DrawLineV(Nodes[prevIdx].Position, GridHandler.SelectedCellCenter, Color.LIME);
            }
        }

        public override void OnMouseEvent(IMouseEvent e)
        {
            if (!isActive) return;

            var (idx, o) = GridHandler.GetCellInfo(e.Position);

            if (o is not Occupant.None && o is not Occupant.TransitNode) return;

            //TODO handle intersections between segments
            if (e is MouseLeftClick mlc)
            {
                if (o is Occupant.None)
                {
                    GridHandler.AddOccupant(idx, Occupant.TransitNode);
                    Nodes.Add(idx, new TransitNode { Idx = idx, Position = GridHandler.SelectedCellCenter });
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
                prevIdx = -1;
            }
        }
    }

    public class TransitNode
    {
        public List<TransitNode> Connections = new();
        public int Idx { get; init; }
        public Vector2 Position { get; set; }
    }
}
