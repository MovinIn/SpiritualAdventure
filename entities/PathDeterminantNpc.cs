using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;
using SpiritualAdventure.levels;
using SpiritualAdventure.utility;

namespace SpiritualAdventure.entities;

[JsonObject(MemberSerialization.OptIn)]
public partial class PathDeterminantNpc : Npc,ICloneable<PathDeterminantNpc>
{
  private bool moving=true;
  private bool repeatMotion;
  private double currTime;
	
  private Replay replay;
  private ReplayPlayer replayPlayer;
  private Vector2 originalPosition;
  
  [JsonProperty]
  private bool isRelativePath;
  [JsonProperty]
  private float moveDelay;
  [JsonProperty]
  private List<MovementAction> actions;

  private const string scenePath = "res://entities/PathDeterminantNpc.cs";
  
  public new static PathDeterminantNpc Instantiate()
  {
    return Npc.Instantiate<PathDeterminantNpc>(scenePath);
  }
	
  private void UpdateReplay()
  {
    originalPosition = Position;
    
    replay = new Replay();
    if (actions.Count==0) return;

    var previousPosition = new Vector2(0,0);
    var past = new TimedInfoFrame(previousPosition, sprite.Scale, 0d);
    replay.add(past);
    foreach (var action in actions)
    {
      double delta = Math.Abs(previousPosition.DistanceTo(action.nextPoint)/SPEED);
      past.scale = sprite.GetScale((action.nextPoint-previousPosition).X);
      sprite.Scale = past.scale;
      past = new TimedInfoFrame(action.nextPoint, past.scale, delta,action.initialDelay);
      previousPosition=action.nextPoint;
      replay.add(past);
    }

    sprite.UpdateRotation(1);
  }

  public PathDeterminantNpc UpdateMovement(List<MovementAction> actions,float moveDelay,
    bool isRelativePath,bool repeatMotion)
  {
    this.actions = actions;
    this.moveDelay = moveDelay;
    this.isRelativePath = isRelativePath;
    this.repeatMotion = repeatMotion;
    currTime = moveDelay;
    
    if (IsInsideTree())
    {
      UpdateReplay();
      replayPlayer.PlayReplay(replay);
    }

    return this;
  }
	
  public override void _EnterTree()
  {
    base._EnterTree();
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
    if (!moving || interactTrigger.isInteracting)
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
      if (repeatMotion)
      {
        ResetReplay();
      }
      else
      {
        IdleOrElse();
      }
      return;
    }
    // Get the relative frame and set animation
    PlayerInfoFrame f=replayPlayer.NextFrame(delta);
    if(isRelativePath) f.position += originalPosition;
    IdleOrElse(Position == f.position?"idle":"walk");
        
    Position = f.position;
    sprite.Scale = f.scale;
  }

  private void ResetReplay()
  {
    currTime = 0;
    replayPlayer.PlayReplay(replay);
    originalPosition = Position;
    IdleOrElse();
  }
  
  public new PathDeterminantNpc Clone()
  {
    return base.Clone().SafelySetScript<PathDeterminantNpc>(scenePath)
      .UpdateMovement(actions.ToList(),moveDelay,isRelativePath,repeatMotion);
  }
}