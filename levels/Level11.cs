using Godot;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.utility;

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
    var devilAppearance = (InlineCutsceneAction)pointers["devilAppearance"];
    var moveDevil1 = (InlineCutsceneAction)pointers["moveDevil1"];
    var moveDevil2 = (InlineCutsceneAction)pointers["moveDevil2"];
    var devilDisappearance = (InlineCutsceneAction)pointers["devilDisappearance"];

    devil.SetDirection(CharacterSprite.Direction.Left);
    devil.Visible = false;
    
    devilAppearance.action = () =>
    {
      devil.Visible = true;
    };

    moveDevil1.action = () =>
    {
      GD.Print("Moving Devil1");
      devil.Position = GameUnitUtils.Vector2(-5, 3);
      devil.SetDirection(CharacterSprite.Direction.Right);
    };
    moveDevil2.action = () =>
    {
      GD.Print("Moving Devil2");
      devil.Position = GameUnitUtils.Vector2(7, -2);
      devil.SetDirection(CharacterSprite.Direction.Left);
    };

    devilDisappearance.action = () =>
    {
      devil.Visible = false;
    };

    LoadLevel();
    NextObjective();
  }
}