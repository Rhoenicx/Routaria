using Routaria.Systems.Chunks;

namespace Routaria.Systems.Snapshots;

public class SnapshotChunk
{
    public required ChunkCoord Coord;
    
    public required bool Stale = true;
    public required uint LastUpdated;
    public required uint Version;
    
    public SnapshotTile[,] Tiles = new SnapshotTile[ChunkSystem.ChunkWidth, ChunkSystem.ChunkHeight];
}