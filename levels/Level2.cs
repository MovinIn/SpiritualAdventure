using System;
using Godot;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level2 : LevelWithTestExtensions
{
  public override void _Ready()
  {
    var npc = PathDeterminantNpc.Instantiate().UpdateMovement(new List<MovementAction>(),
      0,true,false);
    npc.Position = new Vector2(1050, 1050);
    
    var touchObjective= TestTouchObjective(new Vector2(200,200));
    var cutsceneObjective = CutsceneTest(npc);
    LoadLevel(new Vector2(0,0),new List<ObjectiveDisplayGroup>{touchObjective,cutsceneObjective},
      new List<Npc>{npc},new Narrator());
    NextObjective();
  }
}