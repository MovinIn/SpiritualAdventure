using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.utility.parse;

public static class DynamicParserUtils
{
  public static Objective DynamicCloneObjective(JObject objective,DynamicParser parser)
  {
    return parser.DynamicClone<Objective,JObject>(objective, null, 
      o => ObjectiveParseUtils.Parse(o, parser));
  }

  public static Npc DynamicParseNpc(JObject data,DynamicParser parser)
  {
    return parser.DynamicParse<Npc,JObject>(data,null,
      o=>NpcParseUtils.Parse(o,parser));
  }
}