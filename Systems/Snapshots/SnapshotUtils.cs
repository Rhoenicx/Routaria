using System;
using Routaria.Systems.Chunks;
using Terraria;

namespace Routaria.Systems.Snapshots;

public static class SnapshotUtils
{
    public static SnapshotTile GetTile(SnapshotChunk[,] snapshotChunks, int chunkX, int chunkY, int tileX, int tileY)
    {
        // Move the given tile positions based on the chunks sizes.
        // If for example a tileX of -1 is given this means the requested
        // tile is actually inside the snapshot to the left.

        switch (tileX)
        {
            case < 0:
            {
                // Calculate the amount of chunks to move to the left.
                var chunkOffset = Math.Abs(tileX) / ChunkSystem.ChunkWidth + 1;

                // Move the chunk position to the left.
                chunkX -= chunkOffset;
                
                // Convert the position to the other chunk.
                tileX += ChunkSystem.ChunkWidth * chunkOffset;
            }
            break;

            case >= ChunkSystem.ChunkWidth:
            {
                // Calculate the amount of chunks to move to the right.
                var chunkOffset = tileX / ChunkSystem.ChunkWidth;
                
                // Move the chunk position to the right.
                chunkX += chunkOffset;
                
                // Convert the position to the chunk on the right
                tileX -= ChunkSystem.ChunkWidth * chunkOffset;
            }
            break;
        }
        
        switch (tileY)
        {
            case < 0:
            {
                // Calculate the amount of chunks to move to the top.
                var chunkOffset = Math.Abs(tileY) / ChunkSystem.ChunkHeight + 1;

                // Move the chunk position to the top.
                chunkY -= chunkOffset;
                
                // Convert the position to the chunk on the top.
                tileY += ChunkSystem.ChunkHeight * chunkOffset;
            }
            break;

            case >= ChunkSystem.ChunkHeight:
            {
                // Calculate the amount of chunks to move to the bottom.
                var chunkOffset = tileY / ChunkSystem.ChunkHeight;
                
                // Move the chunk position to the bottom.
                chunkY += chunkOffset;
                
                // Convert the position to the chunk on the bottom.
                tileY -= ChunkSystem.ChunkHeight * chunkOffset;
            }
            break;
        }

        // Check the chunk calculated chunk coordinate, if it falls outside 
        // the snapshot arrays it is invalid and should not be used.
        if (chunkX < 0
            || chunkX >= snapshotChunks.GetLength(0)
            || chunkY < 0
            || chunkY >= snapshotChunks.GetLength(1))
        {
            // Invalid
            return new SnapshotTile();
        }
        
        // Otherwise get the correct chunk and tile
        return snapshotChunks[chunkX, chunkY].Tiles[tileX, tileY];
    }
}