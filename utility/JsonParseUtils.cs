using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility;

public static class JsonParseUtils
{

  public static List<Npc> ParseNpcList(JToken json,Node futureParent)
  {
    var npcList = new List<Npc>();
    foreach (var token in json.Values())
    {
      if (token.Type != JTokenType.Object)
      {
        throw new JsonException("Tried to create npc, but type was not JObject.");
      }

      npcList.Add(ParseNpc(token.ToObject<JObject>(),futureParent));
    }

    return npcList;
  }
  
  public static Npc ParseNpc(JObject json,Node futureParent)
  {
	Npc npc;
	if (json["movement"] != null)
	{
      var actionList = ParseMovementActions(json["movement"]);
	  
	  float moveDelay = json.Value<float>("moveDelay");
	  bool isRelativePath = json.OrAsOptional("isRelativePath", true);
	  bool repeatMotion = json.Value<bool>("repeatMotion");
      
	  npc = PathDeterminantNpc.Instantiate().UpdateMovement(actionList,moveDelay,isRelativePath,repeatMotion);
	}
	else
    {
      npc = Npc.Instantiate();
    }

	npc.Position = new Vector2(json["x"].Value<float>(),json["y"].Value<float>());

    futureParent.AddChild(npc);
	
	if (!Enum.TryParse(json.Value<string>("speaker") ?? Speaker.Archer.ToString(), out Speaker speaker))
	{
	  speaker = Speaker.Archer;
	}
    string name = json.OrAsOptional("name", "Narrator");
	npc.Who(speaker,name);

    if (!json["interactable"].Value<bool>() && json["trigger"] == null && json["content"] == null) return npc;
    
    string trigger= json.OrAsOptional("trigger","interact");
    string content = json.OrAsOptional("content", "Talk");
    npc.UseTrigger(trigger,content);

    if (!json.TryGetValue("speech", out var speechToken)) return npc;

    var speechList = ParseSpeechList(speechToken);
    npc.SetSpeech(speechList);

    futureParent.RemoveChild(npc);

    return npc;
  }

  private static List<SpeechLine> ParseSpeechList(JToken speechArray)
  {
    var speechList=new List<SpeechLine>();
    foreach (var token in speechArray.Values<JToken>())
    {
      if (token.Type == JTokenType.String)
      {
        var fileDataToken=ParseTokenFromFile(token.Value<string>());
        AddToSpeechOrThrow(fileDataToken,speechList);
      }
      else
      {
        AddToSpeechOrThrow(token,speechList);
      }
    }

    return speechList;
  }
  
  
  private static List<MovementAction> ParseMovementActions(JToken movements)
  {
    var actions = new List<MovementAction>();
    foreach (var jToken in movements.Values())
    {
      if (jToken.Type != JTokenType.Array)
      {
        throw new JsonException("Movement inside movements must be an array consisting of " +
                                "[(int)x,(int)y,(float)optional{delay}].");
      }
      
      var movement = jToken.ToObject<JArray>();
      switch (movement.Count)
      {
        case 2:
          actions.Add(new MovementAction(movement[0].Value<int>(),movement[1].Value<int>()));
          break;
        case 3:
          actions.Add(new MovementAction(movement[0].Value<int>(),movement[1].Value<int>(),
            movement[2].Value<float>()));
          break;
        default:
          throw new JsonException("Invalid movement jObject inside movements");
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

  private static T OrAsOptional<T>(this JObject json, string key,T optional)
  {
	return json.TryGetValue(key, out var tokenValue) ? tokenValue.ToObject<T>() : optional;
  }

  public static T ParseFromFile<T>(string file) where T:JToken
  {
	using var reader = new StreamReader(file);
	return JToken.Parse(reader.ReadToEnd()).ToObject<T>();
  }

  public static JToken ParseTokenFromFile(string file)
  {
	using var reader = new StreamReader(file);
	return JToken.Parse(reader.ReadToEnd());
  }
  
}
