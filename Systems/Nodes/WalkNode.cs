using Microsoft.Xna.Framework;
using Routaria.Systems.Agents;
using Routaria.Systems.Snapshots;

namespace Routaria.Systems.Nodes;

public class WalkNode : Node
{
    public override void SetStaticDefaults()
    {
        DebugColor = Color.Red;
    }

    public override bool IsValid(Agent agent, SnapshotChunk[,] snapshotChunks, int chunkX, int chunkY, int tileX, int tileY)
    {
        // Check if the given tile position is a solid or top solid tile.
        if (!SnapshotUtils.GetTile(snapshotChunks, chunkX, chunkY, tileX, tileY).IsSolidOrTopSolid)
        {
            // If it is not solid or top solid, exit here
            return false;
        }

        // Variable to count the amount of free columns
        var freeColumns = 0;
        
        // Determine the amount of columns that will be checked
        var remainingColumns = agent.Width * 2 - 1;
        
        // Scan columns from left to right around the node.
        for (var x = -agent.Width + 1; x < agent.Width; x++)
        {
            // Count down the remaining columns (the remaining iterations of the loop)
            remainingColumns--;
            
            // Track if the column is free
            var columnIsFree = true;
            
            // Check the complete height of the agent against this column
            for (var y = 1; y <= agent.Height; y++)
            {
                // Use the special passable logic to figure out if the agent
                // can walk on this tile.
                if (IsPassable(agent, snapshotChunks, chunkX, chunkY, tileX + x, tileY - y))
                {
                    continue;
                }
                
                // In case there is a not-passable tile, invalidate the column. 
                columnIsFree = false;
                break;
            }

            // The entire height of the hitbox fits inside this column.
            if (columnIsFree)
            {
                // Count up the amount of free columns
                freeColumns++;
                
                // When there are enough columns free beside each other
                // to fit the agent's hitbox, consider this a walkable node
                if (freeColumns >= agent.Width)
                {
                    return true;
                }
                
                // Not enough free columns found yet, go to the next one. 
                continue;
            }
            
            // If this column is not free, reset the counter
            freeColumns = 0;
            
            // In case there are not enough remaining columns to fit
            // the agent hitbox, exit early.
            if (remainingColumns < agent.Width)
            {
                break;
            }
        }

        return false;
    }
}