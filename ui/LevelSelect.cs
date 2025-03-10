using Godot;
using SpiritualAdventure.ui;

public partial class LevelSelect : CanvasLayer
{

  private SpiritualAdventure.ui.LevelSelectPopulator populator;
  private Button previous, next;
  
	
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	populator = GetNode<SpiritualAdventure.ui.LevelSelectPopulator>("%LevelFlowPanel");
	previous = GetNode<Button>("%PreviousPageButton");
	next = GetNode<Button>("%NextPageButton");
  }
  
  public void OnMainMenuPressed()
  {
	Root.MainMenu();
  }

  public void LoadLevels()
  {
	var tuple=SpiritualAdventure.ui.LevelSelectPopulator.LoadLevels(SpiritualAdventure.ui.LevelSelectPopulator.Action.None);
	
	previous.Disabled=!tuple.Item1;
	next.Disabled=!tuple.Item2;
  }
  
  public void OnPreviousPagePressed()
  {
	previous.Disabled=!SpiritualAdventure.ui.LevelSelectPopulator.LoadLevels(SpiritualAdventure.ui.LevelSelectPopulator.Action.Previous).Item1;
  }

  public void OnNextPagePressed()
  {
	next.Disabled=!SpiritualAdventure.ui.LevelSelectPopulator.LoadLevels(SpiritualAdventure.ui.LevelSelectPopulator.Action.Next).Item2;
  }
  
}
