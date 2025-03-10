using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility.parse;

public static class NpcParseUtils
{
  private static Npc ParseNpcSkeleton(JObject data,DynamicParser parser)
  {
    dynamic dyn = data;
    string type=(string)dyn.type;
    if (type.ToLower().Contains("path"))
    {
      //I'm a path-determinant npc.
      float moveDelay = dyn.moveDelay ?? 0;
      bool repeatMotion=dyn.repeatMotion ?? false;
      bool isRelativePath = dyn.relativePath ?? true;
      List<MovementAction> movement = MovementActionParseUtils.Parse(
        parser.OrOfPointer<JArray>(dyn.movement,null,out bool _));
      return PathDeterminantNpc.Instantiate().UpdateMovement(movement,moveDelay,isRelativePath,repeatMotion);
    }
    
    //I'm a regular npc.
    return Npc.Instantiate();
  }

  public static Npc Parse(JObject data,DynamicParser parser)
  {
    Npc npc=ParseNpcSkeleton(data,parser);
    dynamic dyn = data;
    npc.Position = GameUnitUtils.Vector2((float)dyn.x, (float)dyn.y);
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
      var line=SpeechParseUtils.Parse(parser.OrOfPointer<JObject>(token,data,out bool isPointer)
        .ToObject<JObject>(),parser);
      if (isPointer) parser.filteredPointers[token.Value<string>()]=line;
      return line;
    }).ToList();
    npc.SetSpeech(speechList);

    NodeStagingArea.Remove(npc);
    return npc;
  }
}