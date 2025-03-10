using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.utility.parse;

public class DynamicParser
{
  public JObject rawPointers { get; private set; }
  public Dictionary<string, object> filteredPointers { get; }

  public DynamicParser(JObject rawPointers)
  {
    this.rawPointers = rawPointers ?? new JObject();
    filteredPointers = new Dictionary<string, object>();
  }
  
  public Level ParseLevel(JObject json,out Level.LevelBuilder skeleton)
  {
    dynamic dyn = json;
    string template=json.Value<string>("template") ?? "Level1";
    var levelScene = ResourceLoader.Load<PackedScene>("res://levels/" + template + ".tscn");
    var level = levelScene.Instantiate<Level>();
    rawPointers = dyn.rawPointers ?? new JObject();

    List<Npc> npcs = ((JArray)dyn.npcs ?? new JArray()).Children().Select(token =>
    {
      Npc npc=DynamicClone<Npc, JObject>(token, null, 
        o => NpcParseUtils.Parse(o, this));
      return npc;
    }).ToList();

    JArray positionArr = dyn.playerPosition ?? new JArray(0,0);
    var playerPosition = GameUnitUtils.Vector2(positionArr[0].Value<float>(), positionArr[1].Value<float>());
    var narrator = new Narrator(IdentityParseUtils.Parse(json));

    List<ObjectiveDisplayGroup> displayGroups = ((JArray)dyn.objectiveDisplayGroups ?? new JArray())
      .Children().Select(token =>
      {
        return DynamicParse<ObjectiveDisplayGroup, JObject>(token,
          null, o => ObjectiveGroupParseUtils.Parse(o, this));
      }).ToList();
    
    foreach (var touchObjective in displayGroups.SelectMany(group=>group.objectives)
               .Where(hasObjective=>hasObjective is TouchObjective))
    {
      level.AddChild((TouchObjective)touchObjective);
    }
    
    skeleton = Level.LevelBuilder.Init()
      .AppendNpcList(npcs)
      .SetPlayerPosition(playerPosition)
      .SetNarrator(narrator)
      .AppendIObjectiveGroups(displayGroups)
      .SetDynamicParser(this);

    return level;
  }

  /**
 * Returns the data of a token that is possibly a pointer.
 * If the token is of type JTokenType.String, then two checks are made: <br/>
 * 1. Checks if `pointers` has a key equivalent to token; if so, it returns pointers[token]. <br/>
 * 2. Checks if the given token is a valid url; if so, it returns the contents of the file.
 */
  public T OrOfPointer<T>(JToken token, JObject extraPointers, out bool isPointer) where T:JToken
  {
    extraPointers ??= new JObject();
    isPointer = token.Type == JTokenType.String;
    if (!isPointer)
    {
      return token.ToObject<T>();
    }

    string pointer = (string)token;
    if (rawPointers.TryGetValue(pointer!, out var pointerToken))
    {
      return OrOfPointer<T>(pointerToken,extraPointers,out _);
    }

    if (extraPointers.TryGetValue(pointer!, out var extraPointerToken))
    {
      return OrOfPointer<T>(extraPointerToken,extraPointers,out _);
    }
    
    if (ResourceLoader.Exists(pointer))
    {
      return ParseFromFile<T>(pointer);
    }

    throw new ArgumentException("token was a string, but did not find a match in " +
                                "pointers, nor was a valid resource path.");
  }

  /**
 * Returns the value of type TV if the token is of type string and is a key in filteredPointers,
 * otherwise outs unparsed JToken of type TP and bool isPointer.
 */
  private TV DynamicParseHelper<TV,TP>(JToken token,JObject extraPointers, 
    out TP unparsed, out bool isUnparsedPointer) where TP:JToken
  {
    unparsed = null;
    isUnparsedPointer = false;
    if (token.Type == JTokenType.String && filteredPointers.TryGetValue(token.Value<string>(), out var value))
    {
      return (TV)value;
    }

    unparsed = OrOfPointer<TP>(token, extraPointers, out isUnparsedPointer);
    return default;
  }

  /**
 * Returns the value of type TV if the token is of type string and is a key in filteredPointers,
 * otherwise uses the parser to return TV. If it uses the parser, it adds the
 * resulting value to filteredPointers.
 */
  public TV DynamicParse<TV, TP>(JToken token,JObject extraPointers,Func<TP,TV> parser) 
    where TP:JToken
  {
    var value = DynamicParseHelper<TV, TP>(token, extraPointers,
      out var unparsed, out var isPointer) ?? parser(unparsed);
    if (isPointer)
    {
      filteredPointers[token.Value<string>()] = value;
    }

    return value;
  }
  /**
 * Similar to DynamicParse except for the fact that it clones the value if it is pre-existing
 * in filteredPointers.
 * <br/><br/>
 * Returns a clone of the value of type TV if the token is of type string and is a key in filteredPointers,
 * otherwise uses the parser to return TV. If it uses the parser, it adds the
 * resulting value to filteredPointers.
 */
  public TV DynamicClone<TV, TP>(JToken token,JObject extraPointers,Func<TP,TV> parser)
    where TP:JToken where TV:class,ICloneable<TV>
  {
    var value = DynamicParseHelper<TV, TP>(token, extraPointers,
      out var unparsed, out var isPointer)?.Clone() ?? parser(unparsed);
    if (isPointer)
    {
      filteredPointers[token.Value<string>()] = value;
    }

    return value;
  }
  
  public static T ParseFromFile<T>(string file) where T:JToken
  {
    using var reader = new StreamReader(file);
    return JToken.Parse(reader.ReadToEnd()).ToObject<T>();
  }
}