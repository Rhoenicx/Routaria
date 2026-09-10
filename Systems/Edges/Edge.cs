using Terraria.ModLoader;

namespace Routaria.Systems.Edges;

public abstract class Edge : ModType
{
    public int Type { get; private set; }

    protected sealed override void Register()
    {
        Type = EdgeSystem.Add(this);
        ModTypeLookup<Edge>.Register(this);
    }
}