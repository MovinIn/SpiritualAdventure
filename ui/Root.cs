using System;
using Godot;
using SpiritualAdventure.levels;

namespace SpiritualAdventure.ui;

public partial class Root : Node
{

  public enum Displaying
  {
    Game,MainMenu,LevelSelect
  }
  
  private static Level currentLevel;
  private static CanvasLayer gameUI,mainMenu;
  private static LevelSelect levelSelect;
  private static int levelCounter;
  private static Root singleton;
  public const int MAX_LEVEL_INDEX = 2;

  public static Displaying currentDisplay = Displaying.MainMenu;
  
  
  public override void _Ready()
  {
    gameUI = GetNode<CanvasLayer>("GameUI");
    levelSelect=GetNode<LevelSelect>("LevelSelect");
    mainMenu = GetNode<CanvasLayer>("MainMenu");
    
    levelCounter = 1;
    singleton = this;
    
    HideAll();
    mainMenu.Visible = true;
    levelSelect.LoadLevels();
  }
  
  public static void RestartLevel()
  {
    LoadLevel();
  }
  
  public static void NextLevel()
  {
    levelCounter++;
    LoadLevel();
  }

  public static void LoadLevel(int level)
  {
    if (level <= 0)
      throw new ArgumentException("level cannot be 0 or less.");
	
    levelCounter = level;
    LoadLevel();
  }

  private static void HideAll()
  {
    gameUI.Visible = false;
    levelSelect.Visible = false;
    mainMenu.Visible = false;
    try
    {
      currentLevel?.QueueFree();
    } catch (ObjectDisposedException){}
  }

  public static void MainMenu()
  {
    HideAll();
    mainMenu.Visible = true;
    currentDisplay = Displaying.MainMenu;
  }

  public static void LevelSelect()
  {
    HideAll();
    levelSelect.Visible = true;
    currentDisplay = Displaying.LevelSelect;
  }

  private static void LoadLevel()
  {
    HideAll();
    gameUI.Visible = true; //TODO: should we re-instantiate or reset?
    
    if (levelCounter > MAX_LEVEL_INDEX)
    {
      LevelSelect();
      return;
    }

    currentDisplay = Displaying.Game;
    var levelScene = ResourceLoader.Load<PackedScene>("res://levels/Level"+levelCounter+".tscn");
    
    currentLevel = levelScene.Instantiate<Level>();
    singleton.AddChild(currentLevel);
    
  }
}