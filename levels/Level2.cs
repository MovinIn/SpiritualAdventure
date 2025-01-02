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
    
    LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup> { touchObjective,cutsceneObjective })
      .AppendNpcList(new List<Npc> { npc })
      .SetNarrator(new Narrator())
      .Build(this);

    LoadLevel();
    
    NextObjective();
  }
}