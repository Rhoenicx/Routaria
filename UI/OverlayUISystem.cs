using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Routaria.Systems.Chunks;
using Routaria.Systems.Nodes;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Routaria.UI;

public class OverlayUISystem : ModSystem
{
    public static NodeChunk[,] NodeChunks;
    
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // Find the interface logic 1 layer, this is the first layer
        var interfaceLogic1Index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));
        
        // Verify the index of the layer
        if (interfaceLogic1Index == -1)
        {
            Mod.Logger.Warn(Name + ": Could not find proper layer to insert after!");
            return;
        }
        
        // Add the Predictions layer below the interface logic layer
        layers.Insert(interfaceLogic1Index, new LegacyGameInterfaceLayer(
            "Routaria: Debug Overlay",
            delegate
            {
                // Draw the layer
                DrawOverlay(Main.spriteBatch);
                return true;
            },
            InterfaceScaleType.Game)
        );
    }

    private void DrawOverlay(SpriteBatch spriteBatch)
    {
        if (NodeChunks == null)
        {
            return;
        }

        var lengthX = NodeChunks.GetLength(0);
        var lengthY = NodeChunks.GetLength(1);
        
        if (lengthX == 0 || lengthY == 0)
        {
            return;
        }

        for (var x = 0; x < lengthX; x++)
        {
            for (var y = 0; y < lengthY; y++)
            {
                DrawChunk(spriteBatch, NodeChunks[x, y]);
            }
        }
    }

    private void DrawChunk(SpriteBatch spriteBatch, NodeChunk chunk)
    {
        var chunkPosition = (new Vector2(chunk.Coord.X, chunk.Coord.Y)
            * new Vector2(ChunkSystem.ChunkWidth, ChunkSystem.ChunkHeight)).ToWorldCoordinates(0f, 0f);
        
        foreach (var kvp in chunk.Nodes)
        {
            foreach (var tilePoint in kvp.Value)
            {
                var position = chunkPosition + new Vector2(tilePoint.X, tilePoint.Y) * 16f;
                
                spriteBatch.Draw(
                    TextureAssets.MagicPixel.Value,
                    position - Main.screenPosition,
                    new Rectangle(0,0, 16,16),
                    kvp.Key.DebugColor * 0.5f);
            }
        }
    }
}