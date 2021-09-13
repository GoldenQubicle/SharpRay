using System.Collections.Generic;
using System.Numerics;

namespace ProtoCity
{
    public class TransitNode
    {
        public List<TransitNode> Connections = new();
        public int Idx { get; init; }
        public Vector2 Position { get; set; }
    }
}
