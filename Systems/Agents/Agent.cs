using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Routaria.Systems.Agents;
using Routaria.Systems.Chunks;
using Routaria.Systems.Nodes;
using Routaria.Systems.Snapshots;
using Routaria.UI;
using Terraria;
using Terraria.ModLoader;

namespace Routaria.Systems.Agents;

public abstract class Agent : ModType
{
    /// <summary>
    /// The type/id of this navigator, this is used internally inside
    /// the pathfinder to group all the NPC's using this navigator type
    /// into one navmesh.
    /// </summary>
    public int Type { get; private set; }

    /// <summary>
    /// The width of the hitbox of this agent type used to calculate valid spaces.
    /// Given in amount of tiles. 1 tile = 16 pixels.
    /// </summary>
    public byte Width { get; protected set; }

    /// <summary>
    /// The height of the hitbox of this agent type used to calculate valid spaces.
    /// Given in amount of tiles. 1 tile = 16 pixels.
    /// </summary>
    public byte Height { get; protected set; }

    public bool OpenDoors { get; protected set; }

    public bool OpenGates { get; protected set; }

    public bool FlyInLiquid { get; protected set; }

    public HashSet<int> AvoidLiquid { get; protected set; } = [];

    /// <summary>
    /// The node types that this agent can use. Only the ones listed here
    /// will be evaluated in the node and navmesh generation.
    /// </summary>
    public List<Node> Nodes { get; protected set; } = [];
    
    /// <summary>
    /// Takes care of registering this navigator to the ModLoader.
    /// </summary>
    protected sealed override void Register()
    {
        Type = AgentSystem.Add(this);
        ModTypeLookup<Agent>.Register(this);
    }

    public virtual Agent Clone()
    {
        var agent = MemberwiseClone();
        return (Agent)agent;
    }

    public sealed override void SetupContent()
    {
        SetStaticDefaults();
    }

    public void RequestPath(Point start, int radius, Action onComplete)
    {
        Task.Run(() => GeneratorLoop(start, radius, onComplete));
    }

    private async void GeneratorLoop(Point start, int radius, Action onComplete)
    {
        try
        {
            // Convert the start to chunk coords
            var coord = start.ToChunkCoord();
            
            // Queue the chunk read request on the main thread, wait for it to complete. 
            await SnapshotSystem.QueueRequest(coord, radius);
            
            // Then get a local copy of the chunks on this worker.
            var snapshotChunks = SnapshotSystem.GetChunks(coord, radius);
            
            // Generate the node graph
            var nodeChunks = NodeSystem.GenerateNodeChunks(this, coord, snapshotChunks);

            OverlayUISystem.NodeChunks = nodeChunks;
            
            // Signal done
            onComplete?.Invoke();
        }
        catch
        {
            // ignored
        }
    }

    #region ===== Navigator Save/Load =================================================
    /*
    public void SaveData(TagCompound tag)
    {
        
    }

    public void LoadData(TagCompound tag)
    {
        
    }
    */
    #endregion
}