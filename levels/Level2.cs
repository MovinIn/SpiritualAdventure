using System.Collections.Generic;
using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.levels;

public partial class Level2 : LevelWithTestExtensions
{
  public override void _Ready()
  {
    // var npc = PathDeterminantNpc.Instantiate().UpdateMovement(new List<MovementAction>(),
    //   0,true,false);
    // npc.Position = new Vector2(1050, 1050);
    
    var touchObjective= TestTouchObjective(new Vector2(200,200));
    // var cutsceneObjective = CutsceneTest(npc);

    AppendBuilder(LevelBuilder.Init()
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup> { touchObjective, /*cutsceneObjective*/ }));
      // .AppendNpcList(new List<Npc> { npc }));
    
    LoadLevel();
    
    NextObjective();
  }
}