using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;

public partial class Level4 : LevelWithTestExtensions
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var objective= TestTouchObjective(new Vector2(100,100));
    
    LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup> { objective })
      .AppendNpcList(new List<Npc>())
      .SetNarrator(new Narrator())
      .Build(this);
    
    LoadLevel();
    
    NextObjective();
  }
}
