using Microsoft.Xna.Framework;

namespace Routaria.Systems.Chunks;

public static class ChunkUtils
{
    public static ChunkCoord ToChunkCoord(this Point point)
    {
        return new ChunkCoord(point.X / ChunkSystem.ChunkWidth, point.Y / ChunkSystem.ChunkHeight);
    }
}
