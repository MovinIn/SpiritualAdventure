using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility;

public static class JsonParseUtils
{
  public static void ParseLevel(JObject json,out Level level,out Level.LevelBuilder dataSkeleton)
  {
    json = DataFromPathIfString(json);
    
    string relativeLevelScenePath=json.Value<string>("template") ?? "Level1";
    
    var levelScene = ResourceLoader.Load<PackedScene>("res://levels/" + relativeLevelScenePath + ".tscn");
    
    level = levelScene.Instantiate<Level>();
    List<Npc> npcs = new();
    if (json.TryGetValue("npcs", out JToken npcData))
    {
      npcs = ParseNpcList(npcData, level,json);
    }

    Vector2 playerPosition;
    
    if (json.ContainsKey("playerPosition")&&json["playerPosition"]!.Type == JTokenType.Array)
    {
      JArray positionArray = json["playerPosition"].ToObject<JArray>();
      playerPosition = new Vector2(positionArray[0].Value<float>(),positionArray[1].Value<float>());
    }
    else
    {
      playerPosition = Vector2.Zero;
    }

    dataSkeleton = Level.LevelBuilder.Init()
      .AppendNpcList(npcs)
      .SetPlayerPosition(playerPosition)
      .SetNarrator(ParseNarrator(json));
  }

  public static Narrator ParseNarrator(JObject json)
  {
    json = DataFromPathIfString(json);
    
    if (!Enum.TryParse(json.Value<string>("speaker") ?? Speaker.Archer.ToString(), out Speaker speaker))
    {
      speaker = Speaker.Archer;
    }
    string name = json.OrAsOptional("name", "Narrator");
    return new Narrator(speaker, name);
  }
  
  public static Objective ParseObjective(JToken json)
  {
    json = DataFromPathIfString(json);
    
    string description=json.Value<string>("description");
    SpeechLine line;
    line = json["feedback"] == null ? null : ParseSpeech(json["feedback"].ToObject<JObject>());
    return new Objective(description, line);
  }

  //TODO: make it return and parse a dictionary
  public static List<Npc> ParseNpcList(JToken json,Node futureParent,JToken rootJson=null)
  {
    json = DataFromPathIfString(json);

    rootJson ??= json;
    
    var npcList = new List<Npc>();
    foreach (var token in json.Values())
    {
      var npcToken = TokenAsDataIfPointer(token,rootJson);
      
      if (npcToken.Type != JTokenType.Object)
      {
        throw new JsonException("Tried to create npc, but type was not JObject.");
      }

      npcList.Add(ParseNpc(npcToken.ToObject<JObject>(),futureParent,rootJson));
    }

    return npcList;
  }
  
  public static Npc ParseNpc(JObject json,Node futureParent,JToken rootJson=null)
  {
    rootJson ??= json;
    
    json = DataFromPathIfString(json);
    
    Npc npc = InitializeNpc(json);

    npc.Position = new Vector2(json.Value<float>("x"),json.Value<float>("y")); //should this be in initializenpc()?

    futureParent.AddChild(npc);
	
    if (!Enum.TryParse(json.Value<string>("speaker") ?? Speaker.Archer.ToString(), out Speaker speaker))
    {
      speaker = Speaker.Archer;
    }
    string name = json.OrAsOptional("name", "Narrator");
    npc.Who(speaker,name);

    if (!json.Value<bool>("interactable") && json["trigger"] == null && json["content"] == null) return npc;
    
    string trigger= json.OrAsOptional("trigger","interact");
    string content = json.OrAsOptional("content", "Talk");
    npc.UseTrigger(trigger,content);

    if (!json.TryGetValue("speech", out var speechToken)) return npc;

    var speechList = ParseSpeechList(speechToken,rootJson);
    npc.SetSpeech(speechList);

    futureParent.RemoveChild(npc);

    return npc;
  }

  private static Npc InitializeNpc(JObject json)
  {
    
    if (json["movement"] == null)
    {
      return Npc.Instantiate();
    }
    
    var actionList = ParseMovementActions(json["movement"],json);
	  
    float moveDelay = json.Value<float>("moveDelay");
    bool isRelativePath = json.OrAsOptional("isRelativePath", true);
    bool repeatMotion = json.Value<bool>("repeatMotion");
      
    return PathDeterminantNpc.Instantiate().UpdateMovement(actionList,moveDelay,isRelativePath,repeatMotion);
  }

  private static List<SpeechLine> ParseSpeechList(JToken speechArrayToken,JToken rootJson)
  {
    var speechList=new List<SpeechLine>();
    foreach (var token in speechArrayToken.Values<JToken>())
    {
      var changeableToken = TokenAsDataIfPointer(token, rootJson);
      AddToSpeechOrThrow(changeableToken,speechList);
    }

    return speechList;
  }
  
  
  private static List<MovementAction> ParseMovementActions(JToken movements,JToken rootJson)
  {
    movements= TokenAsDataIfPointer(movements, rootJson);
    
    var actions = new List<MovementAction>();
    foreach (var jToken in movements.Children())
    {
      if (jToken.Type != JTokenType.Array)
      {
        throw new JsonException("Movement inside movements must be an array consisting of " +
                                "[(int)x,(int)y,(float)optional{delay}].");
      }
      
      try
      {
        var movement = jToken.ToObject<JArray>();
        switch (movement.Count)
        {
          case 2:
            actions.Add(new MovementAction(movement[0].Value<int>(), movement[1].Value<int>()));
            break;
          case 3:
            actions.Add(new MovementAction(movement[0].Value<int>(), movement[1].Value<int>(),
              movement[2].Value<float>()));
            break;
          default:
            throw new JsonException("Invalid movement jObject inside movements");
        }
      }
      catch (JsonException)
      {
        throw new JsonException("Invalid movement data inside movements. Must be an jarray with " +
                                "[int,int] or [int,int,float] describing [x,y,initialDelay].");
      }
    }

    return actions;
  }
  
  private static void AddToSpeechOrThrow(JToken token,List<SpeechLine> speechList)
  {
    switch (token.Type)
    {
      case JTokenType.Array:
        speechList.AddRange(token.ToObject<JArray>().Values<JObject>().Select(ParseSpeech));
        break;
      case JTokenType.Object:
        speechList.Add(ParseSpeech(token.ToObject<JObject>()));
        break;
      default:
        throw new JsonException("Speech token is not an object, nor is it an array.");
    }
  }
  
  public static SpeechLine ParseSpeech(JObject data)
  {
    return ParseSpeechRecursive(data, data, new Dictionary<string, SpeechLine>());
  }
  
  private static SpeechLine ParseSpeechRecursive(JObject root,JObject json,Dictionary<string,SpeechLine> lineCache)
  {
    string content= json.Value<string>("content");
    if (content == null) return null;
	
	
    SpeechLine line = new SpeechLine(content);
	
    if(json.TryGetValue("options", out var optionsToken))
    {
      var optionsDictionary = new Dictionary<string, SpeechLine>();
      foreach (JArray optionTuple in optionsToken.Values<JArray>().ToArray())
      {
        if (optionTuple.Count != 2)
        {
          throw new JsonException("an option of speech with content: \n"+content+"\n does not have 2 values");
        }
        string optionText=optionTuple.First!.Value<string>();
        string speechKey = optionTuple.Last!.Value<string>();
        SpeechLine next;
		
        if (lineCache.TryGetValue(speechKey, out var value))
        {
          next = value;
        }
        else
        {
          next = ParseSpeechRecursive(root,root[speechKey].ToObject<JObject>(), lineCache);
          lineCache.Add(speechKey,next);
        }
		
        optionsDictionary.Add(optionText,next);
      }
	  
      line.options = optionsDictionary;
    }

    if (!json.ContainsKey("nextKey"))
    {
      return line;
    }
	
    string nextKey = json.Value<string>("nextKey");
	
    line.next=ParseSpeechRecursive(root,root[nextKey].ToObject<JObject>(), lineCache);
    lineCache.Add(nextKey,line.next);
	
    return line;
  }

  private static T DataFromPathIfString<T>(T token) where T:JToken
  {
    return token.Type != JTokenType.String ? token : ParseFromFile<T>(token.Value<string>());
  }
  
  private static T TokenAsDataIfPointer<T>(T token,JToken rootJson) where T:JToken
  {
    if (token.Type != JTokenType.String) return token;

    string tokenData = token.ToObject<string>();
    
    if (rootJson.Type == JTokenType.Object && ((JObject)rootJson).ContainsKey(tokenData))
    {
      return ((JObject)rootJson)[tokenData].ToObject<T>();
    }

    if (!ResourceLoader.Exists(tokenData))
    {
      throw new JsonException("Could not find path: " + tokenData);
    }
    
    return ParseFromFile<T>(token.Value<string>());
  }

  private static T OrAsOptional<T>(this JObject json, string key,T optional)
  {
    return json.TryGetValue(key, out var tokenValue) ? tokenValue.ToObject<T>() : optional;
  }

  public static T ParseFromFile<T>(string file) where T:JToken
  {
    using var reader = new StreamReader(file);
    return JToken.Parse(reader.ReadToEnd()).ToObject<T>();
  }
}