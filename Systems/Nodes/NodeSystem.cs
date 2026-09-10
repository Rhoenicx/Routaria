using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Routaria.Systems.Agents;
using Routaria.Systems.Agents;
using Routaria.Systems.Chunks;
using Routaria.Systems.Edges;
using Routaria.Systems.Snapshots;
using Terraria;
using Terraria.ModLoader;

namespace Routaria.Systems.Nodes;

public class NodeSystem : ModSystem
{
    internal static List<Node> Nodes;

    public static int Count => Nodes.Count;

    internal static int Add(Node node)
    {
        Nodes ??= [];
        
        Nodes.Add(node);
        return Nodes.Count - 1;
    }

    public static Node Get(int type) => type < 0 || type >= Count ? null : Nodes[type];

    public static bool TryGet(int type, out Node node)
    {
        node = Get(type);
        return node != null;
    }

    public override void Load()
    {
        Nodes ??= [];
    }

    public override void Unload()
    {
        Nodes = null;
    }
    
    public static NodeChunk[,] GenerateNodeChunks(Agent agent, ChunkCoord coord, SnapshotChunk[,] snapshotChunks)
    {
        var lengthX = snapshotChunks.GetLength(0);
        var lengthY = snapshotChunks.GetLength(1);
        
        var nodeChunks = new NodeChunk[lengthX, lengthY];

        // Loop over the chunks
        for (var chunkX = 0; chunkX < lengthX; chunkX++)
        {
            for (var chunkY = 0; chunkY < lengthY; chunkY++)
            {
                // Create a new node chunk on this position
                var nodeChunk = new NodeChunk { Coord = snapshotChunks[chunkX, chunkY].Coord };
                nodeChunks[chunkX, chunkY] = nodeChunk;
                
                // Loop over all the tiles inside the chunk
                for (var tileX = 0; tileX < ChunkSystem.ChunkWidth; tileX++)
                {
                    for (var tileY = 0; tileY < ChunkSystem.ChunkHeight; tileY++)
                    {
                        // Loop over all the nodes this agent can use
                        foreach (var node in agent.Nodes)
                        {
                            // Check if the node is valid
                            if (!node.IsValid(agent, snapshotChunks, chunkX, chunkY, tileX, tileY))
                            {
                                continue;
                            }

                            // Check if the list with nodes exists, if not create it
                            if (!nodeChunk.Nodes.TryGetValue(node, out var list))
                            {
                                list = [];
                                nodeChunk.Nodes[node] = list;
                            }
                            
                            list.Add(new Point(tileX, tileY));
                        }
                    }
                }
            }
        }

        
        
        return nodeChunks;
    }
}