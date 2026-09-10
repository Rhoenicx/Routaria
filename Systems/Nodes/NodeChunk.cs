using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Routaria.Systems.Chunks;

namespace Routaria.Systems.Nodes;

public class NodeChunk
{
    public required ChunkCoord Coord;
    public Dictionary<Node, List<Point>> Nodes = [];
}