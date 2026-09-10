using Routaria.Systems.Agents;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Routaria.Items;

public class Snapshotter : ModItem
{
    public override string Texture =>  $"Terraria/Images/Item_{ItemID.Wrench}";

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Wrench);
    }

    public override bool CanUseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            ModContent.GetInstance<GroundAgent>().RequestPath(
                player.Center.ToTileCoordinates(),
                2,
                () => { Main.NewText("completed");});
        }

        return base.CanUseItem(player);
    }
}