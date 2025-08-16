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

  public const bool IsDeveloper = true;
  
  private static Level currentLevel;
  private static CanvasLayer gameUI,mainMenu;
  private static LevelSelect levelSelect;
  private static CommandsUI commandsUI;
  private static LevelInfoUI levelInfoUI;
  private static int levelCounter;
  private static Root singleton;
  public const int MAX_LEVEL_INDEX = 13;

  public static Displaying currentDisplay = Displaying.MainMenu;
  
  
  public override void _Ready()
  {
	gameUI = GetNode<CanvasLayer>("GameUI");
	levelSelect=GetNode<LevelSelect>("LevelSelect");
	mainMenu = GetNode<CanvasLayer>("MainMenu");
	commandsUI = GetNode<CommandsUI>("CommandsUI");
	levelInfoUI = GetNode<LevelInfoUI>("LevelInfoUI");
	
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

  public static void DisplayLevelInfo(string title,string description)
  {
	levelInfoUI.Display(title,description);
  }

  public static void CloseLevelInfo()
  {
	levelInfoUI.OnClose();
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
	  var parser = new DynamicParser(null);
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
	if (!IsDeveloper) return;
	
	if (Level.InGame()&&@event.IsActionPressed("printCameraPosition"))
	{
	  Vector2 pixelVector = GetViewport().GetCamera2D().GetScreenCenterPosition();
	  GD.Print("Pixel Vector: "+pixelVector);
	  GD.Print("Game Unit Vector: "+GameUnitUtils.FromPixels(pixelVector));
	  return;
	}

	if (Level.InGame() && @event.IsActionPressed("ghostMode"))
	{
	  Level.SetCameraMode(Level.currentCameraMode!=Level.CameraMode.Ghost 
		? Level.CameraMode.Ghost : Level.CameraMode.Player);
	  GD.Print("toggling to mode: "+Level.currentCameraMode);
	  return;
	}

	if (@event.IsActionPressed("command"))
	{
	  commandsUI.FocusCommandTerminal();
	  return;
	}

	if (@event.IsActionPressed("pause"))
	{
	  PauseSplash.SetPaused(!PauseSplash.Paused());
	  return;
	}
  }
}
