using System;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.levels;
using SpiritualAdventure.utility;
using SpiritualAdventure.utility.parse;

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
  public const int MAX_LEVEL_INDEX = 7;

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
    gameUI.QueueFree();
    var scene = ResourceLoader.Load<PackedScene>("res://ui/GameUI.tscn");
    gameUI = scene.Instantiate<CanvasLayer>();
    singleton.AddChild(gameUI);
    
    
    
    HideAll();
    gameUI.Visible = true; //TODO: should we re-instantiate or reset?
    
    if (levelCounter > MAX_LEVEL_INDEX)
    {
      LevelSelect();
      return;
    }

    currentDisplay = Displaying.Game;

    string jsonPath = "utility/json/Level" + levelCounter + ".json";

    if (ResourceLoader.Exists(jsonPath))
    {
      var parser = new DynamicParser(null); //TODO: somehow get this parser accessible in Level[X].cs
      currentLevel = parser.ParseLevel(DynamicParser.ParseFromFile<JObject>(jsonPath),
        out var dataSkeleton);
      currentLevel.AppendBuilder(dataSkeleton);
    }
    else
    {
      var levelScene = ResourceLoader.Load<PackedScene>("res://levels/Level"+levelCounter+".tscn");
    
      currentLevel = levelScene.Instantiate<Level>();
    }
    
    singleton.AddChild(currentLevel);
    
  }

  public override void _Input(InputEvent @event)
  {
    if (Level.InGame()&&@event.IsActionPressed("printCameraPosition"))
    {
      GD.Print(GetViewport().GetCamera2D().GetScreenCenterPosition());
    }

    if (Level.InGame() && @event.IsActionPressed("ghostMode"))
    {
      Level.SetCameraMode(Level.currentCameraMode!=Level.CameraMode.Ghost 
        ? Level.CameraMode.Ghost : Level.CameraMode.Player);
      GD.Print("toggling to mode: "+Level.currentCameraMode);
    }
  }
}