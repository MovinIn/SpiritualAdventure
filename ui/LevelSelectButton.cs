using Godot;
using SpiritualAdventure.ui;

public partial class LevelSelectButton : Button
{
  
  private static PackedScene scene = ResourceLoader.Load<PackedScene>("res://ui/LevelSelectButton.tscn");
  private int level;

  public static LevelSelectButton Instantiate(int levelNumber)
  {
    var b = scene.Instantiate<LevelSelectButton>();
    b.level = levelNumber;
    b.SetText("" + levelNumber);
    return b;
  }

  public void OnPressed()
  {
    Root.LoadLevel(level);
  }
}
