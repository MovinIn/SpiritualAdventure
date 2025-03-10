using Godot;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.levels;

public partial class Level11 : Level
{
  public override void _Ready()
  {

    var pointers=builder.parser.filteredPointers;
    foreach (var filteredPointersKey in builder.parser.filteredPointers.Keys)
    {
      GD.Print(filteredPointersKey);
    }
    var devil = (Npc)pointers["devil"];
    InlineCutsceneAction devilAppearance, moveDevil1, moveDevil2;
    devilAppearance = (InlineCutsceneAction)pointers["devilAppearance"];
    moveDevil1 = (InlineCutsceneAction)pointers["moveDevil1"];
    moveDevil2 = (InlineCutsceneAction)pointers["moveDevil2"];

    devilAppearance.action = () =>
    {
      devil.Visible = true;
    };

    moveDevil1.action = () =>
    {
      GD.Print("Moving Devil1");
    };
    moveDevil2.action = () =>
    {
      GD.Print("Moving Devil2");
    };

    LoadLevel();
    NextObjective();
  }
}