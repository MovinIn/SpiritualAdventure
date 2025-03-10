using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.utility.parse;

public static class MovementActionParseUtils
{
  public static List<MovementAction> Parse(JArray movements)
  {
    var movementActions=new List<MovementAction>();
    foreach (JToken token in movements)
    {
      JArray actionData = (JArray)token;
      double delay=actionData.Count==3 ? actionData[2].ToObject<double>() : 0;
      var movementAction = new MovementAction(
        GameUnitUtils.Vector2(actionData[0].Value<int>(), actionData[1].Value<int>()),delay);
      movementActions.Add(movementAction);
    }

    return movementActions;
  } 
}