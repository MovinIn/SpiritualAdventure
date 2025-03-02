using Godot;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.utility;
using SpiritualAdventure.utility.parse;

public partial class Level1 : LevelWithTestExtensions
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	var objective= TestTouchObjective(new Vector2(100,100));
	var parser = new DynamicParser(null);
	var parsedNpc = NpcParseUtils.Parse(DynamicParser.ParseFromFile<JObject>("utility/json/NpcTest.json"),parser);    

	var builder=LevelBuilder.Init()
	  .SetPlayerPosition(Vector2.Zero)
	  .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup> { objective })
	  .AppendNpcList(new List<Npc> { parsedNpc })
	  .SetNarrator(new Narrator());
	
	AppendBuilder(builder);
	
	LoadLevel();
	NextObjective();
  }
}
