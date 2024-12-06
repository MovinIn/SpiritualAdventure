using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;

public partial class Level4 : Level
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var objective= TestTouchObjective(new Vector2(100,100));
    LoadLevel(new Vector2(0,0),new List<IHasObjective>{objective},new List<Npc>(),new Narrator());
    NextObjective();
  }

  public IHasObjective TestTouchObjective(Vector2 position,int timeLimit=-1)
  {
    var touchObjective=TouchObjective.Instantiate(ObjectiveBuilder.TimedOrElse(
      "Touch the Checkpoint at "+position,timeLimit));
    touchObjective.Position = position;
    AddChild(touchObjective);
    return touchObjective;
  }
}
