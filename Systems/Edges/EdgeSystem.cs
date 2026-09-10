using System.Collections.Generic;
using Terraria.ModLoader;

namespace Routaria.Systems.Edges;

public class EdgeSystem : ModSystem
{
    internal static List<Edge> Edges;

    public static int Count => Edges.Count;

    internal static int Add(Edge edge)
    {
        Edges ??= [];
        
        Edges.Add(edge);
        return Edges.Count - 1;
    }

    public static Edge Get(int type) => type < 0 || type >= Count ? null : Edges[type];

    public static bool TryGet(int type, out Edge edge)
    {
        edge = Get(type);
        return edge != null;
    }

    public override void Load()
    {
        Edges ??= [];
    }

    public override void Unload()
    {
        Edges = null;
    }
}