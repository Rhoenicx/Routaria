using System.Collections.Generic;
using Routaria.Systems.Agents;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Routaria.Systems.Agents;

public class AgentSystem : ModSystem
{
    /// <summary>
    /// A list holding all the registered agent types.
    /// </summary>
    internal static List<Agent> Agents;

    /// <summary>
    /// Gets the total amount of agents loaded.
    /// </summary>
    public static int Count => Agents.Count;
    
    /// <summary>
    /// Adds the given agent to the internal list.
    /// Called when an agent is found and loaded by tML.
    /// </summary>
    internal static int Add(Agent agent)
    {
        Agents ??= [];
        
        Agents.Add(agent);
        return Agents.Count - 1;
    }

    /// <summary>
    /// Gets the agent of the given type. If the type
    /// does not exist, null will be returned.
    /// Consider using ModContent.GetInstance instead.
    /// </summary>
    public static Agent Get(int type) => type < 0 || type >= Count ? null : Agents[type];

    /// <summary>
    /// Tries to get the agent of the given type. If
    /// an agent exists, true will be returned and the
    /// agent can be accessed by the out argument.
    /// Returns false if the agent does not exist.
    /// Consider using ModContent.GetInstance instead.
    /// </summary>
    public static bool TryGet(int type, out Agent agent)
    {
        agent = Get(type);
        return agent != null;
    }

    /// <summary>
    /// Initializes the collections when the mod loads
    /// </summary>
    public override void Load()
    {
        Agents ??= [];
    }

    /// <summary>
    /// Clears all the collections when the mod unloads
    /// </summary>
    public override void Unload()
    {
        Agents = null;
    }

    /*
    public override void SaveWorldData(TagCompound tag)
    {
        foreach (var agent in Agents)
        {
            var navTag = new TagCompound();
            agent.SaveData(navTag);
            tag.Add(agent.FullName, navTag);
        }
    }

    public override void LoadWorldData(TagCompound tag)
    {
        foreach (var agent in Agents)
        {
            if (tag.ContainsKey(agent.FullName))
            {
                agent.LoadData(tag.GetCompound(agent.FullName));
            }
        }
    }
    */
}