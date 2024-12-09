using Godot;
using System;
using SpiritualAdventure.levels;
using SpiritualAdventure.ui;

public partial class PauseSplash : MarginContainer
{
  private static Button restart, menu, next,cancel;
  private static RichTextLabel description;
  private static bool buttonsEnabled;
  
  private static PauseSplash singleton;
  
  public enum State
  {
	Paused,Complete,Failed
  }
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	singleton = this; 
	Visible = false;
	restart = GetNode<Button>("%Restart");
	menu = GetNode<Button>("%MainMenu");
	next = GetNode<Button>("%NextLevel");
    cancel = GetNode<Button>("%Cancel");
    description = GetNode<RichTextLabel>("%PauseDescription");
	buttonsEnabled = false;
  }

  public static void Display(State state)
  {
    
    switch (state)
    {
      case State.Complete:
        description.Text = "[center][color=006800]LEVEL COMPLETE![/color][/center]";
        break;
      case State.Failed:
        description.Text = "[center][color=ff0037]LEVEL FAILED![/color][/center]";
        break;
      case State.Paused:
        description.Text = "[center][color=000000]PAUSED[/color][/center]";
        break;
    }
    
    
	next.Visible = true;
	menu.Visible = true;
	restart.Visible = true;
    cancel.Visible = true;
    
	buttonsEnabled = true;

	if (state is State.Paused or State.Failed)
	{
	  next.Visible = false;
	}

    if (state is State.Complete or State.Failed)
    {
      cancel.Visible = false;
    }

	singleton.Visible = true;
  }

  public static bool Paused()
  {
	return singleton.Visible;
  }

  public void RestartPressed()
  {
	if (!ButtonsEnabled()) return;
	Visible = false;
    
	Root.RestartLevel();
  }
  
  public void MainMenuPressed()
  {
	if (!ButtonsEnabled()) return;
	Visible = false;

	Root.MainMenu();
  }
  
  public void NextLevelPressed()
  {
	if (!ButtonsEnabled()) return;
	Visible = false;
    
	Root.NextLevel();
  }

  public void CancelPressed()
  {
    if (!ButtonsEnabled()) return;
    Visible = false;
  }

  private bool ButtonsEnabled()
  {
	if (!buttonsEnabled)
	{
	  Visible = false;
	  return false;
	}
	
	buttonsEnabled = true;
	return true;
  }
  
}
