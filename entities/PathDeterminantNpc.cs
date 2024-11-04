using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.utility;

namespace SpiritualAdventure.entities;

public partial class PathDeterminantNpc : Npc
{
	private static PackedScene scene;
	
	
	private bool moving,isRelativePath;
	private float MOVE_DELAY;
	private double currTime;
	
	private Replay replay;
	private ReplayPlayer replayPlayer;
	private Vector2 originalPosition;
	private List<Vector2> path;

	static PathDeterminantNpc()
	{
		scene = ResourceLoader.Load<PackedScene>("res://entities/npc.tscn");
	}

	public static PathDeterminantNpc instance(List<Vector2> path,Vector2 position,float moveDelay,bool isRelativePath)
	{
		Npc instance=scene.Instantiate<Npc>();
		PathDeterminantNpc npc=instance.SafelySetScript<PathDeterminantNpc>("res://entities/PathDeterminantNpc.cs");
		npc.path = path;
		npc.MOVE_DELAY = moveDelay;
		npc.currTime = moveDelay;
		npc.isRelativePath = isRelativePath;
		npc.Position = position;
		return npc;
	}
	
	private void updateReplay()
	{
		replay = new Replay();
		if (path.Count==0) return;

		TimedInfoFrame past = new TimedInfoFrame(path[0], sprite.Scale, 0d);
		replay.add(past);
		for (int i = 1; i < path.Count; i++)
		{
			double delta = Math.Abs(path[i - 1].DistanceTo(path[i])/SPEED);
			past.scale = sprite.getScale((path[i]-path[i-1]).X);
            sprite.Scale = past.scale;
			past = new TimedInfoFrame(path[i], past.scale, delta);
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
		updateReplay();
        replayPlayer.playReplay(replay);
	}

	public void setMoving(bool moving)
    {
		this.moving = moving;
	}
	
	public override void _PhysicsProcess(double delta)
    {
		// If not moving, or interacting, or in delay, do not process.
        if (!moving || interactTrigger.IsInteracting())
        {
            idleOrElse();
            return;
        }
		if (currTime<MOVE_DELAY)
		{
			currTime += delta;
			return;
		}
		
		if (!replayPlayer.hasNextFrame())
		{
			currTime = 0;
			replayPlayer.playReplay(replay);
			originalPosition = Position;
            idleOrElse();
			return;
		}
		// Get the relative frame
		PlayerInfoFrame f=replayPlayer.nextFrame(delta);
		if(isRelativePath) f.position += originalPosition;
        if (Position != f.position)
        {
            idleOrElse("walk");
        }
		Position = f.position;
		sprite.Scale = f.scale;
	}
}
