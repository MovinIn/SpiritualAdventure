using Godot;
using SpiritualAdventure.cutscene.actions;

namespace SpiritualAdventure.levels;

public partial class Level13 : Level
{
  public override void _Ready()
  {
    var inline=(InlineCutsceneAction)builder.parser.filteredPointers["inline-pointer"];
    inline.action = () =>
    {
      player.Position = new Vector2(250, 250);
    };

    LoadLevel();
    NextObjective();
  }
}