using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Routaria.Systems.Chunks;
using Terraria;
using Terraria.ModLoader;

namespace Routaria.Systems.Snapshots;

public class SnapshotSystem : ModSystem
{
    private const int MaxRequestPerTick = 5;

    private static ConcurrentQueue<SnapshotRequest> _requests;
    
    private static SnapshotChunk[,] _snapshotChunks;
    
    public override void Load()
    {
        _requests = [];
    }

    public override void Unload()
    {
        _requests = null;
        _snapshotChunks = null;
    }

    public override void ClearWorld()
    {
        _requests.Clear();
    }

    public override void PostWorldLoad()
    {
        // Create a new snapshot array to save all the chunked snapshots
        _snapshotChunks = new SnapshotChunk[ChunkSystem.ChunkAmountX, ChunkSystem.ChunkAmountY];
    }

    public override void PostUpdateEverything()
    {
        var regenAmount = 0;

        while (!_requests.IsEmpty)
        {
            if (!_requests.TryDequeue(out var request))
            {
                break;
            }
            
            if (CreateSnapshot(request.Coord) == SnapshotResult.Regenerated)
            {
                regenAmount++;
            }

            request.Source?.SetResult();
            
            if (regenAmount >= MaxRequestPerTick)
            {
                break;
            }
        }
    }

    public static Task QueueRequest(ChunkCoord coord, int radius)
    {
        return QueueRequest(new ChunkCoord(coord.X - radius, coord.Y - radius), radius * 2 + 1, radius * 2 + 1);
    }

    public static Task QueueRequest(ChunkCoord coord, int width, int height, int padding = 1)
    {
        // Make sure the width and height are always at least 1
        width = Math.Max(1, width);
        height = Math.Max(1, height);
        
        // Add additional padding, this is handy for generating the nodes so on 1 by default.
        if (padding > 0)
        {
            coord = new ChunkCoord(coord.X - padding, coord.Y - padding);
            width += padding * 2;
            height += padding * 2;
        }

        SnapshotRequest request = null;
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var newCoord = new ChunkCoord(coord.X + x, coord.Y + y);

                if (!newCoord.Valid)
                {
                    continue;
                }

                request = new SnapshotRequest(newCoord);
                _requests.Enqueue(request);
            }
        }

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        
        if (request != null)
        {
            request.Source = completion;
        }
        else
        {
            completion.SetResult();
        }

        return completion.Task;
    }
    
    private static SnapshotResult CreateSnapshot(ChunkCoord coord)
    {
        // Check if the given coord is inside the world
        if (!coord.Valid)
        {
            // If that is not the case, terminate here and do not generate.
            return SnapshotResult.OutsideWorld;
        }
        
        // Get the current snapshot
        var snapshot = _snapshotChunks[coord.X, coord.Y];
        
        // If the snapshot already exists execute additional checks
        if (snapshot is { Stale: false } && snapshot.LastUpdated + 600 >= Main.GameUpdateCount)
        {
            return SnapshotResult.NotNeeded;
        }
        
        // Create a new snapshot object
        snapshot = new SnapshotChunk
        {
            Coord = snapshot?.Coord ?? coord,
            LastUpdated = Main.GameUpdateCount,
            Stale = false,
            Version = snapshot != null ? snapshot.Version + 1 : 0
        };
        
        // Create a copy of the world tiles
        for (var x = 0; x < ChunkSystem.ChunkWidth; x++)
        {
            for (var y = 0; y < ChunkSystem.ChunkHeight; y++)
            {
                var wx = x + coord.X * ChunkSystem.ChunkWidth;
                var wy = y + coord.Y * ChunkSystem.ChunkHeight;
                
                if (wx < 0 || wx >= Main.maxTilesX || wy < 0 || wy >= Main.maxTilesY)
                {
                    snapshot.Tiles[x, y] = new SnapshotTile { Valid = false };
                    continue;
                }

                var tile = Main.tile[wx, wy];

                snapshot.Tiles[x, y] = new SnapshotTile
                {
                    Valid = true,
                    HasTile = tile.HasTile,
                    TileType = tile.TileType,
                    IsActuated = tile.IsActuated,
                    WallType = tile.WallType,
                    BlockType = (byte)tile.BlockType,
                    LiquidType = (byte)tile.LiquidType,
                    LiquidAmount = tile.LiquidAmount,
                    FrameY = tile.TileFrameY,
                };
            }
        }
        
        _snapshotChunks[coord.X, coord.Y] = snapshot;

        return SnapshotResult.Regenerated;
    }

    public static SnapshotChunk[,] GetChunks(ChunkCoord coord, int radius)
    {
        return GetChunks(new ChunkCoord(coord.X - radius, coord.Y - radius), radius * 2 + 1, radius * 2 + 1);
    }
    
    public static SnapshotChunk[,] GetChunks(ChunkCoord coord, int width, int height)
    {
        // Make sure there are no negative numbers
        width = Math.Max(1, width);
        height = Math.Max(1, height);

        // Make sure the coord always starts at 0,0
        var startX = Math.Max(0, coord.X);
        var startY = Math.Max(0, coord.Y);
        
        // Limit the end to the chunk amounts inside the world
        var endX = Math.Min(ChunkSystem.ChunkAmountX, coord.X + width);
        var endY = Math.Min(ChunkSystem.ChunkAmountY, coord.Y + height);
        
        // The requested rectangle doesn't intersect the world
        if (startX >= endX || startY >= endY)
        {
            return new SnapshotChunk[0, 0];
        }

        // Get the actual size of the rectangle inside the world
        var resultWidth = endX - startX;
        var resultHeight = endY - startY;

        // Create the new 2d array for the chunks
        var chunks = new SnapshotChunk[resultWidth, resultHeight];
        
        // Copy the chunk references from the cache to the worker
        for (var x = 0; x < resultWidth; x++)
        {
            for (var y = 0; y < resultHeight; y++)
            {
                chunks[x, y] = _snapshotChunks[startX + x, startY + y];
            }
        }
        
        return chunks;
    }
    
    private class SnapshotRequest(ChunkCoord coord)
    {
        public ChunkCoord Coord { get; } = coord;
        [CanBeNull] public TaskCompletionSource Source;
    }
    
    private enum SnapshotResult
    {
        OutsideWorld,
        NotNeeded,
        Regenerated,
    }
}

