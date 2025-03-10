using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.utility.parse;

namespace SpiritualAdventure.ui;

public partial class LevelSelectPopulator : FlowContainer
{
  private const int levelBatchAmount = 15;
  private static LevelSelectPopulator singleton;

  private static int lowerEndLevelIndexBound = Math.Min(levelBatchAmount, Root.MAX_LEVEL_INDEX);
  
  private int endLevelIndex = lowerEndLevelIndexBound;
  private readonly Dictionary<string, Tuple<string, string>> levelInfoMap = ParseLevelInfo();
  
  public enum Action
  {
    Previous=-1,None=0,Next=1
  }

  public override void _Ready()
  {
    singleton = this;
  }

  /**
 * Loads the next N=levelBatchAmount levels, stopping at maxLevelIndex. Returns if previous button & next button
 * should be enabled respectively.
 */
  public static Tuple<bool,bool> LoadLevels(Action action)
  {
    singleton.endLevelIndex = Math.Max(lowerEndLevelIndexBound,
      Math.Min(singleton.endLevelIndex + (int)action*levelBatchAmount - 1, Root.MAX_LEVEL_INDEX));
    int beginLevelIndex = Math.Max(1, singleton.endLevelIndex - levelBatchAmount);
	
    Clear();
    for (int i = beginLevelIndex; i <= singleton.endLevelIndex; i++)
    {
      var button = LevelSelectButton.Instantiate(i);
      if (singleton.levelInfoMap.TryGetValue("Level"+i, out var info))
      {
        button.WithInfo(info.Item1, info.Item2);
      }
      singleton.AddChild(button);
    }

    return new Tuple<bool,bool>(beginLevelIndex!=1,singleton.endLevelIndex!=Root.MAX_LEVEL_INDEX);
  }

  private static void Clear()
  {
    foreach (Node n in singleton.GetChildren())
    {
      n.QueueFree();
    }
  }

  private static Dictionary<string, Tuple<string, string>> ParseLevelInfo()
  {
    Dictionary<string, Tuple<string, string>> map=new();
    JObject json = DynamicParser.ParseFromFile<JObject>("utility/json/LevelInfo.json");
    foreach (var property in json.Properties())
    {
      string key = property.Name;
      JObject value = (JObject)property.Value;
      map[key] = new Tuple<string, string>(value.Value<string>("title"), value.Value<string>("description"));
    }

    return map;
  }
}