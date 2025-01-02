using System.Collections.Generic;
using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.levels;

public class LevelLoadData
{
  public Vector2 playerPosition = Vector2.Zero;
  public List<ObjectiveDisplayGroup> iObjectiveGroups = new();
  public List<Npc> npcList = new();
  public Narrator narrator = new();
}