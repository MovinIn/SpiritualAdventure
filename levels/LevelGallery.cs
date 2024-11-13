using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.levels;

public static class LevelGallery
{
  
  public static Level LoadLevel(string path)
  {
    List<Npc> npcList=new();
    List<Objective> objectives=new();
    var jObject=JObject.Parse(ResourceLoader.Load<string>(path));
    if(jObject.TryGetValue("npcs", out var npcToken))
    {
      foreach (var npc in npcToken as JArray)
      {
        var npcPath=npc.Value<string>("path") ?? "res://entities/Npc.tscn";
        npcList.Add(Npc.Instantiate<Npc>(npcPath));
      }
    }
    if(jObject.TryGetValue("objectives", out var objectiveToken))
    {
      foreach (var objective in objectiveToken as JArray)
      {
        objectives.Add(objective.ToObject<Objective>());
      }
    }

    var narrator = jObject.Value<Narrator>("narrator") ?? new Narrator();

    Level level = new();
    level.LoadLevel(objectives,npcList,narrator);
    return level;
  }

  public static Level LoadLevel(int index)
  {
    return LoadLevel("res://levels/data/Level"+index+".json");
  }
}