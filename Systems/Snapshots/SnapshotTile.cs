using Routaria.Systems.Agents;
using Terraria;
using Terraria.ID;

namespace Routaria.Systems.Snapshots;

public struct SnapshotTile
{
    public bool Valid;
    public bool HasTile;
    public bool IsActuated;
    public ushort TileType;
    public ushort WallType;
    public byte BlockType;
    public byte LiquidType; // Once tML implements ModLiquid, change to ushort or int.
    public byte LiquidAmount;
    public short FrameY;
    
    public bool IsSolidOrTopSolid => Valid 
        && HasTile 
        && !IsActuated 
        && (Main.tileSolid[TileType] 
            || (Main.tileSolidTop[TileType] && FrameY == 0));

    public bool IsNotSolid => Valid
        && (!HasTile 
            || IsActuated 
            || !Main.tileSolid[TileType] 
            || Main.tileSolidTop[TileType]);
}