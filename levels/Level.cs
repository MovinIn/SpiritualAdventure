using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;
using SpiritualAdventure.utility;
using SpiritualAdventure.utility.parse;

namespace SpiritualAdventure.levels;

[JsonObject(MemberSerialization.OptIn)]
public partial class Level : Node2D
{
  public class LevelBuilder
  {
    public Vector2 playerPosition = Vector2.Zero;
    public List<ObjectiveDisplayGroup> iObjectiveGroups { get; private set; }
    public List<Npc> npcList { get; private set; }
    public Narrator narrator { get; private set; }
    public List<GameObject> gameObjects { get; private set; }
    public DynamicParser parser { get; private set; }

    private LevelBuilder()
    {
      iObjectiveGroups = new List<ObjectiveDisplayGroup>();
      npcList = new List<Npc>();
      narrator = new Narrator();
      gameObjects = new List<GameObject>();
      parser = new DynamicParser(null);
    }

    public static LevelBuilder Init()
    {
      return new LevelBuilder();
    }

    public LevelBuilder SetPlayerPosition(Vector2 playerPosition)
    {
      this.playerPosition = playerPosition;
      return this;
    }

    public LevelBuilder SetDynamicParser(DynamicParser parser)
    {
      this.parser = parser;
      return this;
    }

    public LevelBuilder AppendGameObjects(List<GameObject> gameObjects)
    {
      this.gameObjects.AddRange(gameObjects);
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

    public LevelBuilder AppendBuilder(LevelBuilder toAppend)
    {
      playerPosition = toAppend.playerPosition;
      AppendNpcList(toAppend.npcList);
      AppendIObjectiveGroups(toAppend.iObjectiveGroups);
      narrator= toAppend.narrator;
      parser = toAppend.parser;
      
      return this;
    }
  }

  public enum CameraMode
  {
    Player,Cutscene,Ghost
  }
  
  
  protected LevelBuilder builder;
  
  protected Queue<ObjectiveDisplayGroup> objectiveQueue=new();
  protected Dictionary<Type, List<Npc>> npcs=new();
  protected Narrator narrator=new();

  public static Player player;

  public static Camera2D cutsceneCamera,ghostCamera;
  public static CameraMode currentCameraMode { get; private set; }

  protected Level()
  {
    builder = LevelBuilder.Init();
  }
  
  public void AppendBuilder(LevelBuilder toAppend)
  {
    builder.AppendBuilder(toAppend);
  }

  private void Init(Vector2 playerPosition,List<ObjectiveDisplayGroup> iObjectiveGroups,List<Npc> npcList,Narrator narrator)
  {
    currentCameraMode=CameraMode.Player;
    
    GetNodeOrNull<TileMapLayer>("InvisibleTileMap")?.SetVisible(false);
    
    cutsceneCamera = new Camera2D();
    ghostCamera = new Camera2D();
    
    AddChild(cutsceneCamera);
    AddChild(ghostCamera);
      
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
    Init(builder.playerPosition, builder.iObjectiveGroups, builder.npcList, builder.narrator);
    
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
      objectiveGroup.requiredObjectives.ForEach(io=>io.objective.FailedObjective());
    });

    foreach (var iObjective in objectiveGroup.requiredObjectives)
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
        
        var line = objective.postCompletionFeedback;
        if (line != null)
        {
          narrator.Narrate(line);
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

  public static void SetCameraMode(CameraMode cameraMode)
  {
    if (!InGame()) return;

    currentCameraMode = cameraMode;
    
    switch(cameraMode)
    {
      case CameraMode.Player:
        player.MakeCameraCurrent();
        break;
      case CameraMode.Ghost:
        ghostCamera.MakeCurrent();
        break;
    }
  }
  
}