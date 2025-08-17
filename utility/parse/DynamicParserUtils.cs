using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.utility.parse;

public static class DynamicParserUtils
{
  public static Objective DynamicCloneObjective(JToken objective,DynamicParser parser)
  {
    return parser.DynamicClone<JObject,Objective>(objective, null, 
      o => ObjectiveParseUtils.Parse(o, parser));
  }

  public static Npc DynamicParseNpc(JToken data,DynamicParser parser)
  {
    return parser.DynamicParse<JObject,Npc>(data,null,
      o=>NpcParseUtils.Parse(o,parser));
  }
}