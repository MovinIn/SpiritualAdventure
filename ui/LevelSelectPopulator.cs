using Godot;
using System;
using SpiritualAdventure.ui;

public partial class LevelSelectPopulator : FlowContainer
{
  private const int levelBatchAmount = 15;
  private static LevelSelectPopulator singleton;

  private static int lowerEndLevelIndexBound = Math.Min(levelBatchAmount, Root.MAX_LEVEL_INDEX);
  
  private int endLevelIndex = lowerEndLevelIndexBound;

  public enum Action
  {
    Previous=-1,None=0,Next=1
  }

  public override void _Ready()
  {
    singleton = this;
  }

  /**
   * Loads the next N=levelBatchAmount levels, stopping at maxLevelIndex. Returns if previous button & next button
   * should be enabled respectively.
   */
  public static Tuple<bool,bool> LoadLevels(Action action)
  {
    
    singleton.endLevelIndex = Math.Max(lowerEndLevelIndexBound,
      Math.Min(singleton.endLevelIndex + (int)action*levelBatchAmount - 1, Root.MAX_LEVEL_INDEX));
    int beginLevelIndex = Math.Max(1, singleton.endLevelIndex - levelBatchAmount);
    
    Clear();
    GD.Print(beginLevelIndex);
    GD.Print(singleton.endLevelIndex);
    for (int i = beginLevelIndex; i <= singleton.endLevelIndex; i++)
    {
      GD.Print("adding a button!");
      var button = LevelSelectButton.Instantiate(i);
      singleton.AddChild(button);
    }

    return new Tuple<bool,bool>(beginLevelIndex!=1,singleton.endLevelIndex!=Root.MAX_LEVEL_INDEX);
  }

  private static void Clear()
  {
    foreach (Node n in singleton.GetChildren())
    {
      n.QueueFree();
    }
  }
}