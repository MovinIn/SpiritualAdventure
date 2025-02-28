using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.cutscene.actions;

namespace SpiritualAdventure.utility.parse;

public static class FlashParseUtils
{
  
  private delegate Action ParseFlashAction(JObject data);
  private static readonly Dictionary<string, ParseFlashAction> flashMap;

  static FlashParseUtils()
  {
    flashMap = new Dictionary<string, ParseFlashAction>
    {
      {"set", data => { return () => {Flash.Set(ParseColor(data)); }; } },
      {"toColor", data => {return () => 
        Flash.ToColor(ParseColor(data), data.Value<float>("duration"));
      } },
      {"stayStagnant", data => { return () =>
        Flash.StayStagnant(data.Value<float>("duration")); 
      } },
      {"dissolve", data => { return () =>
        Flash.Dissolve(data.Value<float>("duration")); 
      } },
      {"toSolid", data => { return () =>
        Flash.ToSolid(data.Value<float>("duration")); 
      } },
      {"initiate",_=>Flash.Initiate},
      {"queueReset",_=>Flash.QueueReset},
      {"hardReset",_=>Flash.HardInstantReset}
    };
  }
  
  public static InlineCutsceneAction Parse(JArray data)
  {
    List<Action> flashActions=data.Children().Select(token =>
    {
      JObject o = token.ToObject<JObject>();
      string type = o.Value<string>("type");
      return flashMap[type].Invoke(o);
    }).ToList();
    
    return new InlineCutsceneAction(() =>
    {
      flashActions.ForEach(action=>action.Invoke());
    });
  }

  private static Color ParseColor(JObject data)
  {
    JToken t=data["color"];
    switch (t.Type)
    {
      case JTokenType.Array:
        JArray a = (JArray)t;
        if (a.Count!=3||a.Count!=4)
        {
          throw new ArgumentException("Failed to parse color, expected length of color array " +
                                      "to be 3 or 4, but was <"+a.Count+">.");
        }

        if (a.Count == 3)
        {
          return new Color(a[0].Value<float>(), a[1].Value<float>(), a[2].Value<float>());
        }
        
        return new Color(a[0].Value<float>(), a[1].Value<float>(), 
          a[2].Value<float>(),a[3].Value<float>());
      
      case JTokenType.String:
        string c = t.Value<string>();
        if (!OpenColors.GetColors().TryGetValue(c,out var color))
        {
          throw new ArgumentException("Failed to parse color, could not parse string: <" + c + ">.");
        }
        return color;
      default:
        throw new ArgumentException("Failed to parse color, expected type JArray or string," +
                                    " but got: <"+t.Type+">.");
    }
  }
}