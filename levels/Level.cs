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
  
  private Queue<IHasObjective> iObjectives=new();
  private Dictionary<Type, List<Npc>> npcs=new();
  private Narrator narrator;

  private bool startObjectives;

  public static Player player;

  public static bool isCutscene { get; private set; }
  public static Camera2D cutsceneCamera;
  
  private void Init(Vector2 playerPosition,List<IHasObjective> iObjectivesList,List<Npc> npcList,Narrator narrator)
  {
    GetNodeOrNull<TileMapLayer>("InvisibleTileMap")?.SetVisible(false);
    
    cutsceneCamera = new Camera2D();
    AddChild(cutsceneCamera);
    
    player = Player.Instantiate();
    player.Position = playerPosition;
    AddChild(player);
    
    this.narrator = narrator;
    narrator.NotInteracting = NextObjective;
    
    foreach (var iObjective in iObjectivesList)
    {
      iObjectives.Enqueue(iObjective);
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
    if (iObjectives.Count == 0) {
      PauseSplash.Display(PauseSplash.State.Complete);
      return; 
    }

    var iObjective = iObjectives.Peek();
    iObjective.objective.AddChangeHandler(OnObjectiveStatusChanged);
    iObjective.objective.SetAsObjective();
    iObjective.Start();
  }

  public void OnObjectiveStatusChanged(Objective.Status status,Objective objective)
  {
    switch (status)
    {
      case Objective.Status.Completed:
      {
        GD.Print("Objective Complete!");
        var lines = iObjectives.Peek().objective.postCompletionFeedback;
        iObjectives.Dequeue();
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
      case Objective.Status.Initiated:
        GD.Print("Starting New Objective!");
        //TODO: do something
        break;
    }
  }

  public static bool Paused() //TODO: should this method be in here?
  {
    return !InGame() || PauseSplash.Paused();
  }
  
  public static bool InGame()
  {
    return Root.currentDisplay == Root.Displaying.Game;
  }

  public static void SetCutscene(bool isCutscene,Vector2 position=default)
  {
    if (!InGame()) return; 
    
    Level.isCutscene = isCutscene;
    
    if (!isCutscene)
    {
      player.MakeCameraCurrent();
    }
    
  }
}