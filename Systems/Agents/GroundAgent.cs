using Routaria.Systems.Agents;
using Routaria.Systems.Nodes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Routaria.Systems.Agents;

public class GroundAgent : Agent
{
    public override void SetStaticDefaults()
    {
        // Hitbox settings
        Width = 2;
        Height = 3;
        OpenDoors = true;
        
        // Nodes that the agent can use. TODO: remove fly from ground agent.
        Nodes.Add(ModContent.GetInstance<WalkNode>());
        Nodes.Add(ModContent.GetInstance<FlyNode>());
        
        // Liquids to avoid, by default lava
        AvoidLiquid.Add(LiquidID.Lava);
    }
}