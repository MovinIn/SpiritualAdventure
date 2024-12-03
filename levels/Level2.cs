using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;

public partial class Level2 : Level
{
  public override void _Ready()
  {
    GD.Print("loading objective");
    var objective= TestTouchObjective(new Vector2(200,200));
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
