using Godot;

namespace SpiritualAdventure.ui;

public partial class LevelSelect : CanvasLayer
{

  private LevelSelectPopulator populator;
  private Button previous, next;
  
	
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    populator = GetNode<LevelSelectPopulator>("%LevelFlowPanel");
    previous = GetNode<Button>("%PreviousPageButton");
    next = GetNode<Button>("%NextPageButton");
  }
  
  public void OnMainMenuPressed()
  {
    Root.MainMenu();
  }

  public void LoadLevels()
  {
    var tuple=LevelSelectPopulator.LoadLevels(LevelSelectPopulator.Action.None);
	
    previous.Disabled=tuple.Item1;
    next.Disabled=tuple.Item2;
  }
  
  public void OnPreviousPagePressed()
  {
    previous.Disabled=LevelSelectPopulator.LoadLevels(LevelSelectPopulator.Action.Previous).Item1;
  }

  public void OnNextPagePressed()
  {
    next.Disabled=LevelSelectPopulator.LoadLevels(LevelSelectPopulator.Action.Next).Item2;
  }
  
}