using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility;

public class DynamicParser
{
  private JObject rawPointers;
  private Dictionary<string, object> filteredPointers;
  public DynamicParser(JObject rawPointers)
  {
    this.rawPointers = rawPointers ?? new JObject();
    filteredPointers = new Dictionary<string, object>();
  }
  
  private SpeechLine ParseSpeechLineRecursive(JObject data,JObject extraPointers)
  {
    dynamic dyn = data;
    var line = new SpeechLine((string)dyn.content);
    if (dyn.nextKey != null)
    {
      line.next = DynamicParse<SpeechLine, JObject>(dyn.nextKey, extraPointers,
        out JObject unparsed, out bool isPointer) ?? ParseSpeechLineRecursive(unparsed, extraPointers);
      // JObject nextKey = OrOfPointer<JObject>(dyn.nextKey,extraPointers,out bool isPointer);
      // line.next = ParseSpeechLineRecursive(nextKey,extraPointers);
      if (isPointer)
      {
        filteredPointers[((JToken)dyn.nextKey).Value<string>()]=line.next;
      }
    }

    if (dyn.options != null)
    {
      JArray options = dyn.options;
      Dictionary<string, SpeechLine> optionsDict=new();
      foreach (var token in options)
      {
        JArray option = (JArray)token;
        SpeechLine nextLine=DynamicParse<SpeechLine,JObject>(option[1],extraPointers,
          out var unparsed,out bool isPointer) ?? ParseSpeechLineRecursive(unparsed, extraPointers);
        // JObject lineData=OrOfPointer<JObject>(option[1],extraPointers,out var isPointer);
        // SpeechLine nextLine = ParseSpeechLineRecursive(lineData,extraPointers);
        if (isPointer)
        {
          filteredPointers[option[1].Value<string>()] = nextLine;
        }
        optionsDict.Add(option[0].Value<string>(),nextLine);
      }
      line.options=optionsDict;
    }

    return line;
  }
  
  public SpeechLine ParseSpeechLine(JObject data)
  {
    return ParseSpeechLineRecursive(data,data);
  }

  private static List<MovementAction> ParseMovementActions(JArray movements)
  {
    var movementActions=new List<MovementAction>();
    foreach (JToken token in movements)
    {
      JArray actionData = (JArray)token;
      double delay=actionData.Count==3 ? actionData[2].ToObject<double>() : 0;
      var movementAction = new MovementAction(actionData[0].Value<int>(), actionData[1].Value<int>(),delay);
      movementActions.Add(movementAction);
    }

    return movementActions;
  } 
  
  private Npc ParseNpcSkeleton(JObject data)
  {
    dynamic dyn = data;
    string type=(string)dyn.type;
    if (type.ToLower().Contains("path"))
    {
      //I'm a path-determinant npc.
      float moveDelay = dyn.moveDelay;
      bool repeatMotion=dyn.repeatMotion ?? false;
      bool isRelativePath = dyn.relativePath ?? true;
      List<MovementAction> movement = ParseMovementActions(
        OrOfPointer<JArray>(dyn.movement,null,out bool _));
      return PathDeterminantNpc.Instantiate().UpdateMovement(movement,moveDelay,isRelativePath,repeatMotion);
    }
    
    //I'm a regular npc.
    return Npc.Instantiate();
  }

  public Npc ParseNpc(JObject data)
  {
    Npc npc=ParseNpcSkeleton(data);
    dynamic dyn = data;
    npc.Position = new Vector2((float)dyn.x, (float)dyn.y);
    NodeStagingArea.Add(npc);
    
    if (!Enum.TryParse((string)dyn.speaker, out Speaker speaker))
    {
      speaker = Speaker.Archer;
    }
    string name = dyn.name ?? "unknown";
    npc.Who(speaker,name);

    if (dyn.interactable == null && dyn.trigger == null && dyn.content == null)
    {
      NodeStagingArea.Remove(npc);
      return npc;
    }
    
    string trigger= dyn.trigger ?? "interact";
    string content = dyn.content ?? "Talk";
    npc.UseTrigger(trigger,content);

    if (dyn.speech==null)
    {
      NodeStagingArea.Remove(npc);
      return npc;
    }

    //TODO: would likely be useful to encapsulate into external method.
    var speechList = ((JArray)dyn.speech).Children().Select(token =>
    {
      var line=ParseSpeechLine(OrOfPointer<JObject>(token,data,out bool isPointer)
        .ToObject<JObject>());
      if (isPointer) filteredPointers[token.Value<string>()]=line;
      return line;
    }).ToList();
    npc.SetSpeech(speechList);

    NodeStagingArea.Remove(npc);
    return npc;
  }
  
  public static Narrator ParseNarrator(JObject json)
  {
    if (!Enum.TryParse(json.Value<string>("speaker") ?? Speaker.Archer.ToString(), out Speaker speaker))
    {
      speaker = Speaker.Archer;
    }
    string name = json.OrAsOptional("name", "Narrator");
    return new Narrator(speaker, name);
  }  

  public Level ParseLevel(JObject json,out Level.LevelBuilder skeleton)
  {
    dynamic dyn = json;
    string template=json.Value<string>("template") ?? "Level1";
    var levelScene = ResourceLoader.Load<PackedScene>("res://levels/" + template + ".tscn");

    rawPointers = dyn.rawPointers ?? new JObject();

    List<Npc> npcs = ((JArray)dyn.npcs ?? new JArray()).Children().Select(token =>
    {
      Npc npc=ParseNpc(OrOfPointer<JObject>(token, null,out bool isPointer));
      if (isPointer)
      {
        filteredPointers[token.Value<string>()]=npc;
      }
      return npc;
    }).ToList();

    JArray positionArr = dyn.playerPosition ?? new JArray(0,0);
    Vector2 playerPosition = new Vector2(positionArr[0].Value<float>(), positionArr[1].Value<float>());
    Narrator narrator = ParseNarrator(json);
    
    skeleton = Level.LevelBuilder.Init()
      .AppendNpcList(npcs)
      .SetPlayerPosition(playerPosition)
      .SetNarrator(narrator);

    //TODO: add preloading of speechlines and npcs, so we can use them inside level!
    return levelScene.Instantiate<Level>();
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
      return pointerToken.ToObject<T>();
    }

    if (extraPointers.TryGetValue(pointer!, out var extraPointerToken))
    {
      return extraPointerToken.ToObject<T>();
    }
    
    if (ResourceLoader.Exists(pointer))
    {
      return ParseFromFile<T>(pointer);
    }

    throw new ArgumentException("token was a string, but did not find a match in " +
                                "pointers, nor was a valid resource path.");
  }

  public TV DynamicParse<TV,TP>(JToken token,JObject extraPointers, out TP unparsed, out bool isPointer) where TP:JToken
  {
    unparsed = null;
    isPointer = false;
    if (token.Type == JTokenType.String && filteredPointers.TryGetValue(token.Value<string>(), out var value))
    {
      return (TV)value;
    }

    unparsed = OrOfPointer<TP>(token, extraPointers, out var x);
    isPointer = x;
    return default;
  }
  
  public static T ParseFromFile<T>(string file) where T:JToken
  {
    using var reader = new StreamReader(file);
    return JToken.Parse(reader.ReadToEnd()).ToObject<T>();
  }
}