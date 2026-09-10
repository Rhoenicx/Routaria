using Microsoft.Xna.Framework;
using Routaria.Systems.Agents;
using Routaria.Systems.Snapshots;
using Terraria.ID;
using Terraria.ModLoader;

namespace Routaria.Systems.Nodes;

public abstract class Node : ModType
{
    public int Type { get; private set; }

    public Color DebugColor { get; protected set; }

    protected sealed override void Register()
    {
        Type = NodeSystem.Add(this);
        ModTypeLookup<Node>.Register(this);
    }

    public sealed override void SetupContent()
    {
        SetStaticDefaults();
    }

    /// <summary>
    /// Checks if the given tile snapshot position can be considered as this node.
    /// Here you would put all the logic needed for determining if the given agent
    /// can occupy/use this node. For example: A walk node checks if there is enough
    /// free space for the agent to stand on. Return true if the agent can use this node.
    /// </summary>
    public abstract bool IsValid(Agent agent, SnapshotChunk[,] snapshotChunks, int chunkX, int chunkY, int tileX, int tileY);
    
    protected static bool IsPassable(Agent agent, SnapshotChunk[,] snapshotChunks, int chunkX, int chunkY, int tileX, int tileY)
    {
        // Get the tile on the given position
        var tile = SnapshotUtils.GetTile(snapshotChunks, chunkX, chunkY, tileX, tileY);

        // Check agent liquid constraints
        if (tile.LiquidAmount > 0 && agent.AvoidLiquid.Contains(tile.LiquidType))
        {
            return false;
        }

        // In case the tile type is an open door or open gate:
        // - The space does not contain any other solid tile.
        // - A open door or gate is always not-solid.
        // => It is safe to assume that open doors and open gates are always passable
        if (tile.TileType is TileID.OpenDoor or TileID.TallGateOpen)
        {
            return true;
        }

        // The tile type on the given position is a closed door.
        if (tile.TileType == TileID.ClosedDoor)
        {
            // In this case first check if the agent is allowed to open doors.
            // If the agent can't open doors, it is safe to assume the closed 
            // door as a solid tile.
            if (!agent.OpenDoors)
            {
                return false;
            }

            // If the agent is able to open doors, check if the door can actually
            // be opened. Here we check if there is space left or right of the door
            // is completely free.
            return TryOpenDoor(snapshotChunks, chunkX, chunkY, tileX, tileY);
        }

        // The tile is an open tall gate, this tile is always non-solid.
        // Assume it is passible if the agent is allowed to open gates.
        if (tile.TileType == TileID.TallGateClosed && agent.OpenGates)
        {
            return true;
        }

        return tile.IsNotSolid;
    }

    protected static bool TryOpenDoor(SnapshotChunk[,] snapshotChunks, int chunkX, int chunkY, int tileX, int tileY)
    {
        // NOTE: For the door opening logic we assume that the agent can open the door
        // in both directions. The checks are done to both sides of the door, if one side
        // is clear it may be opened.
        
        // A closed terraria door is 1 tile wide and 3 tiles tall. We actually don't
        // know in this code which tile of the three we're given. To make the check work,
        // first find the bottom piece of the door by scanning downwards, this will be the
        // start position.
        
        // Scan downwards for max 2 tiles, assuming the given tile position is already one
        // of the 3 door pieces.
        for (var y = 0; y < 2; y++)
        {
            // Get the potential door piece one tile down.
            var potentialDoorTile = SnapshotUtils.GetTile(snapshotChunks, chunkX, chunkY, tileX, tileY + 1);

            // If there is no door piece, exit early.
            if (potentialDoorTile.TileType != TileID.ClosedDoor)
            {
                break;
            }
            
            // Otherwise move the y position down
            tileY++;
        }
        
        // From here we have the bottom position of the closed door.
        
        // Scan the column on the left and right side of the door.
        // Skip the middle column (this is the column with the door).
        for (var x = -1; x <= 1; x += 2)
        {
            // Track if the door can be opened
            var canOpen = true;

            // Check the 3 tiles besides the door
            for (var y = 0; y < 3; y++)
            {
                // Get the tile besides the door
                var tile = SnapshotUtils.GetTile(snapshotChunks, chunkX, chunkY, tileX + x, tileY - y);

                // Check if the tile position is clear.
                // TODO: check if a door can overlap with actuated tiles!
                // TODO: check if a door can be actuated!
                if (tile.Valid && tile is not { HasTile: true, IsActuated: false })
                {
                    continue;
                }
                
                // The tile is occupied: door cannot open.
                canOpen = false;
                
                // Exit early
                break;
            }

            // Check if the door can open in this column
            if (canOpen)
            {
                // If the can open, exit early.
                return true;
            }
        }

        // No clear space found.
        return false;
    }
}