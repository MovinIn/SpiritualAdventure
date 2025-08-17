using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.utility.parse;

namespace SpiritualAdventure.levels;

public partial class Level1 : LevelWithTestExtensions
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var objective= TestTouchObjective(new Vector2(100,100));
    var parser = new DynamicParser(null);
    var parsedNpc = NpcParseUtils.Parse(DynamicParser.ParseFromFile<JObject>("utility/json/NpcTest.json"),parser);    

    AppendBuilder(LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup> { objective })
      .AppendNpcList(new List<Npc> { parsedNpc })
      .SetNarrator(new Narrator()));
	
    LoadLevel();
    NextObjective();
  }
}