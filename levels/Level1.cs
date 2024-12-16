using Godot;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.utility;

public partial class Level1 : LevelWithTestExtensions
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var objective= TestTouchObjective(new Vector2(100,100));
    var parsedNpc=JsonParseUtils.ParseNpc(JsonParseUtils.ParseFromFile<JObject>("utility/json/NpcTest.json"),this);
    LoadLevel(new Vector2(0,0),new List<ObjectiveDisplayGroup>{objective},
      new List<Npc>{parsedNpc},new Narrator());
    NextObjective();
  }
}