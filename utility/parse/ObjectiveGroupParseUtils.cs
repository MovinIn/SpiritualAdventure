using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.utility.parse;

public static class ObjectiveGroupParseUtils
{
  public static ObjectiveDisplayGroup Parse(JObject data,DynamicParser parser)
  {
    foreach (var token in (data["hiddenObjectives"] ?? new JArray()).Children())
    {
      HasObjectiveParseUtils.Parse(parser.OrOfPointer<JObject>(token, null, out _), parser);
    }
    
    List<IHasObjective> objectives = data["objectives"].Children().Select(token => HasObjectiveParseUtils.Parse(
      parser.OrOfPointer<JObject>(token,null,out _),parser)).ToList();
    return new ObjectiveDisplayGroup(objectives,data.Value<float>("timeLimit"));
  }
}