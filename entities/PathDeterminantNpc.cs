using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.levels;
using SpiritualAdventure.utility;

namespace SpiritualAdventure.entities;

[JsonObject(MemberSerialization.OptIn)]
public partial class PathDeterminantNpc : Npc
{
  private bool moving;
  private double currTime;
	
  private Replay replay;
  private ReplayPlayer replayPlayer;
  private Vector2 originalPosition;
  
  [JsonProperty]
  private bool isRelativePath;
  [JsonProperty]
  private float moveDelay;
  [JsonProperty]
  private List<Action> actions;
  
  public override void Parse(JObject json)
  {
    base.Parse(json);
    if (json.TryGetValue("moveDelay", out var delayToken))
    {
      currTime = moveDelay;
      moveDelay=delayToken.ToObject<float>();
    }
    if (json.TryGetValue("isRelativePath", out var isRelativePathToken))
    {
      isRelativePath = isRelativePathToken.ToObject<bool>();
    }
    
    actions=json.Value<List<Action>>("actions") ?? new List<Action>();
  }
  
  public static PathDeterminantNpc Instantiate(List<Action> actions,Vector2 position,float moveDelay,bool isRelativePath)
  {
    var npc=Npc.Instantiate(position)
      .SafelySetScript<PathDeterminantNpc>("res://entities/PathDeterminantNpc.cs");
    GD.Print("finalized npc instantiation");
    npc.actions = actions;
    npc.moveDelay = moveDelay;
    npc.currTime = moveDelay;
    npc.isRelativePath = isRelativePath;
    return npc;
  }
	
  private void UpdateReplay()
  {
    replay = new Replay();
    if (actions.Count==0) return;

    var previousPosition = new Vector2(0,0);
    var past = new TimedInfoFrame(previousPosition, sprite.Scale, 0d);
    replay.add(past);
    foreach (var action in actions)
    {
      double delta = Math.Abs(previousPosition.DistanceTo(action.nextPoint)/SPEED);
      past.scale = sprite.getScale((action.nextPoint-previousPosition).X);
      sprite.Scale = past.scale;
      past = new TimedInfoFrame(action.nextPoint, past.scale, delta,action.initialDelay);
      previousPosition=action.nextPoint;
      replay.add(past);
    }

    sprite.updateRotation(1);
  }
	
  public override void _Ready()
  {
    base._Ready();
    originalPosition = Position;
    moving = true;
    replayPlayer = new ReplayPlayer();
    UpdateReplay();
    replayPlayer.PlayReplay(replay);
  }

  public void SetMoving(bool moving)
  {
    this.moving = moving;
  }
	
  public override void _PhysicsProcess(double delta)
  {
    if (Level.Paused()) return;
    
    // If not moving, or interacting, or in delay, do not process.
    if (!moving || interactTrigger.IsInteracting())
    {
      IdleOrElse();
      return;
    }
    if (currTime<moveDelay)
    {
      currTime += delta;
      return;
    }
		
    if (!replayPlayer.HasNextFrame())
    {
      currTime = 0;
      replayPlayer.PlayReplay(replay);
      originalPosition = Position;
      IdleOrElse();
      return;
    }
    // Get the relative frame and set animation
    PlayerInfoFrame f=replayPlayer.NextFrame(delta);
    if(isRelativePath) f.position += originalPosition;
    IdleOrElse(Position == f.position?"idle":"walk");
        
    Position = f.position;
    sprite.Scale = f.scale;
  }
}