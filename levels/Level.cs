using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.levels;

[JsonObject(MemberSerialization.OptIn)]
public partial class Level : Node2D
{
  
  private Queue<ObjectiveDisplayGroup> objectiveQueue=new();
  private Dictionary<Type, List<Npc>> npcs=new();
  private Narrator narrator;

  private bool startObjectives;

  public static Player player;

  public static bool isCutscene { get; private set; }
  public static Camera2D cutsceneCamera;
  
  private void Init(Vector2 playerPosition,List<ObjectiveDisplayGroup> iObjectiveGroups,List<Npc> npcList,Narrator narrator)
  {
    GetNodeOrNull<TileMapLayer>("InvisibleTileMap")?.SetVisible(false);
    
    cutsceneCamera = new Camera2D();
    AddChild(cutsceneCamera);
    
    player = Player.Instantiate();
    player.Position = playerPosition;
    AddChild(player);
    
    this.narrator = narrator;
    narrator.NotInteracting = CheckAllCompleted;
    
    foreach (var objectiveGroup in iObjectiveGroups)
    {
      objectiveQueue.Enqueue(objectiveGroup);
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
  
  
  
  public void LoadLevel(Vector2 playerPosition,List<ObjectiveDisplayGroup> iObjectiveGroups,List<Npc> npcList,Narrator narrator)
  {
    Init(playerPosition,iObjectiveGroups,npcList,narrator);
    foreach (var npc in npcList)
    {
      AddChild(npc);
    }
  }

  public void NextObjective()
  {
    if (objectiveQueue.Count == 0) {
      PauseSplash.Display(PauseSplash.State.Complete);
      return; 
    }

    var objectiveGroup = objectiveQueue.Peek();
    
    ObjectiveDisplay.UpdateObjective(objectiveGroup, () =>
    {
      objectiveGroup.objectives.ForEach(io=>io.objective.FailedObjective());
    });

    foreach (var iObjective in objectiveGroup.objectives)
    {
      iObjective.objective.AddChangeHandler(OnObjectiveStatusChanged);
      iObjective.objective.SetAsObjective();
      iObjective.Start();
    }
  }

  public void CheckAllCompleted()
  {
    if (!objectiveQueue.Peek().AllCompleted()) return;
    
    objectiveQueue.Dequeue();
    NextObjective();
  }
  

  public void OnObjectiveStatusChanged(Objective.Status status,Objective objective)
  {
    switch (status)
    {
      case Objective.Status.Completed:
      {
        GD.Print("Objective Complete!");
        
        var lines = objective.postCompletionFeedback;
        if (lines != null)
        {
          narrator.Narrate(lines);
        }
        else
        {
          CheckAllCompleted();
        }
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