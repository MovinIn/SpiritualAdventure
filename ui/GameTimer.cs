using System.Collections.Generic;
using Godot;
using SpiritualAdventure.levels;
using Action=System.Action;

namespace SpiritualAdventure.ui;

public partial class GameTimer: Node
{
  private static PriorityQueue<Action, double> actions;
  private static double currTime;

  public override void _Ready()
  {
	actions = new PriorityQueue<Action, double>();
	currTime = 0;
  }


  public override void _Process(double delta)
  {
	if (Level.Paused()) return;

	if (actions.Count==0)
	{
	  currTime = 0;
	  return;
	}

	currTime += delta;
	if(!actions.TryPeek(out _,out double lowestDelay))
	{
	  return;
	}
	
	while (currTime > lowestDelay)
	{
	  actions.Dequeue().Invoke();
	  
	  if (!actions.TryPeek(out _, out double newLowestDelay))
	  {
		return;
	  }
	  lowestDelay = newLowestDelay;
	}
	
  }

  public static void Add(Action callbackAction,double delay)
  {
	actions.Enqueue(callbackAction,delay+currTime);
  }
  
}
