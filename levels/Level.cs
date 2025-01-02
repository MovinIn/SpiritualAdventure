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
  protected class LevelBuilder
  {
    private Vector2 playerPosition = Vector2.Zero;
    private List<ObjectiveDisplayGroup> iObjectiveGroups = new();
    private List<Npc> npcList = new();
    private Narrator narrator = new();

    private LevelBuilder() { }

    public static LevelBuilder Init()
    {
      return new LevelBuilder();
    }

    public LevelBuilder SetPlayerPosition(Vector2 playerPosition)
    {
      this.playerPosition = playerPosition;
      return this;
    }
    
    public LevelBuilder AppendIObjectiveGroups(List<ObjectiveDisplayGroup> iObjectiveGroups)
    {
      this.iObjectiveGroups.AddRange(iObjectiveGroups);
      return this;
    }
    
    public LevelBuilder AppendNpcList(List<Npc> npcList)
    {
      this.npcList.AddRange(npcList);
      return this;
    }

    public LevelBuilder ClearNpcList()
    {
      npcList.Clear();
      return this;
    }

    public LevelBuilder ClearIObjectiveGroups()
    {
      iObjectiveGroups.Clear();
      return this;
    }
    
    public LevelBuilder SetNarrator(Narrator narrator)
    {
      this.narrator = narrator;
      return this;
    }

    public void Build(Level level)
    {
      level.Init(playerPosition, iObjectiveGroups, npcList, narrator);
    }
  }
  
  
  private Queue<ObjectiveDisplayGroup> objectiveQueue=new();
  private Dictionary<Type, List<Npc>> npcs=new();
  private Narrator narrator;

  private bool startObjectives;

  public static Player player;

  public static bool isCutscene { get; private set; }
  public static Camera2D cutsceneCamera;

  private void Init(Vector2 playerPosition,List<ObjectiveDisplayGroup> iObjectiveGroups,List<Npc> npcList,Narrator narrator)
  {
    isCutscene = false;
    
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
  
  
  
  public void LoadLevel()
  {
    foreach (var npcList in npcs.Values)
    {
      npcList.ForEach(npc=>AddChild(npc));
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

  public static void SetCutscene(bool isCutscene)
  {
    if (!InGame()) return; 
    
    Level.isCutscene = isCutscene;
    
    if (!isCutscene)
    {
      player.MakeCameraCurrent();
    }
    
  }
}