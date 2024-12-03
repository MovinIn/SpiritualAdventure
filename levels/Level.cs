using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.levels;

[JsonObject(MemberSerialization.OptIn)]
public partial class Level : Node2D
{
  
  private Queue<Objective> objectives=new();
  
  private List<IHasObjective> iObjectives=new();
  private Dictionary<Type, List<Npc>> npcs=new();
  private Narrator narrator;

  private bool startObjectives;

  public static Player player;
  
  private void Init(Vector2 playerPosition,List<IHasObjective> iObjectivesList,List<Npc> npcList,Narrator narrator)
  {
    player = Player.Instantiate();
    player.Position = playerPosition;
    AddChild(player);
    
    this.narrator = narrator;
    narrator.NotInteracting = NextObjective;
    
    foreach (var iObjective in iObjectivesList)
    {
      iObjectives.Add(iObjective);
      objectives.Enqueue(iObjective.objective);
    }
    
    foreach (var npc in npcList)
    {
      if (!npcs.ContainsKey(npc.GetType()))
      {
        npcs.Add(npc.GetType(),new List<Npc>());
      }
      
      npcs[npc.GetType()].Add(npc);
    }
  }
  
  public void LoadLevel(Vector2 playerPosition,List<IHasObjective> iObjectivesList,List<Npc> npcList,Narrator narrator)
  {
    Init(playerPosition,iObjectivesList,npcList,narrator);
    foreach (var npc in npcList)
    {
      AddChild(npc);
    }
  }

  public void NextObjective()
  {
    if (objectives.Count == 0) {
      PauseSplash.Display(PauseSplash.State.Complete);
      return; 
    }
    objectives.Peek().AddChangeHandler(OnObjectiveStatusChanged);
    objectives.Peek().SetAsObjective();
  }

  public void OnObjectiveStatusChanged(Objective.Status status,Objective objective)
  {
    switch (status)
    {
      case Objective.Status.Completed:
      {
        GD.Print("Objective Complete!");
        var lines = objectives.Peek().postCompletionFeedback;
        objectives.Dequeue();
        if (lines == null)
        {
          NextObjective();
          return;
        }
        narrator.Narrate(lines);
        break;
      }
      case Objective.Status.Failed:
        GD.Print("Objective Failed!");
        PauseSplash.Display(PauseSplash.State.Failed);
        break;
      case Objective.Status.Start:
        GD.Print("Starting New Objective!");
        //TODO: do something
        break;
    }
  }

  public static bool Paused() //TODO: should this method be in here?
  {
    return Root.currentDisplay != Root.Displaying.Game || PauseSplash.Paused();
  }
}