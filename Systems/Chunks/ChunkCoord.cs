namespace Routaria.Systems.Chunks;

public readonly struct ChunkCoord(int x, int y)
{
    public readonly int X = x;
    public readonly int Y = y;

    /// <summary>
    /// Checks if the chunk is valid: if it inside the bounds
    /// of the current loaded world.
    /// </summary>
    public bool Valid => X >= 0 && X < ChunkSystem.ChunkAmountX && Y >= 0 && Y < ChunkSystem.ChunkAmountY;
}