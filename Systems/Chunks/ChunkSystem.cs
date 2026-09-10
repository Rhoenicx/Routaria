using Terraria;
using Terraria.ModLoader;

namespace Routaria.Systems.Chunks;

public class ChunkSystem : ModSystem
{
    /// <summary>
    /// The amount of tiles on the X axis inside one chunk
    /// </summary>
    public const int ChunkWidth = 32;
    
    /// <summary>
    /// The amount of tiles on the Y axis inside one chunk
    /// </summary>
    public const int ChunkHeight = 32;
    
    /// <summary>
    /// The amount of chunks on the X axis inside the current world
    /// </summary>
    public static int ChunkAmountX { get; private set; }
    
    /// <summary>
    /// The amount of chunks on the Y axis inside the current world
    /// </summary>
    public static int ChunkAmountY { get; private set; }

    /// <summary>
    /// Called after the world has loaded from disk and is ready to be entered.
    /// Main.tile is ready for reads.
    /// </summary>
    public override void PostWorldLoad()
    {
        // Calculate the amount of needed chunks for this world.
        ChunkAmountX = Main.maxTilesX / ChunkWidth + (Main.maxTilesX % ChunkWidth != 0 ? 1 : 0);
        ChunkAmountY = Main.maxTilesY / ChunkHeight + (Main.maxTilesY % ChunkHeight != 0 ? 1 : 0);
    }
}