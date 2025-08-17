using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.utility.parse;

public static class ObjectiveGroupParseUtils
{
  public static ObjectiveDisplayGroup Parse(JObject data,DynamicParser parser)
  {
    List<IHasObjective> hiddenObjectives = (data["hiddenObjectives"] ?? new JArray()).Children().Select(token=>
      HasObjectiveParseUtils.Parse(parser.OrOfPointer<JObject>(token, null, out _), parser)).ToList();
    
    List<IHasObjective> requiredObjectives = data["objectives"].Children().Select(token => 
      HasObjectiveParseUtils.Parse(parser.OrOfPointer<JObject>(token,null,out _),parser)).ToList();
    
    return ObjectiveDisplayGroup.Builder.Init(requiredObjectives)
      .AddHiddenObjectives(hiddenObjectives)
      .WithTimeLimit(data.Value<float>("timeLimit")).Build();
  }
}