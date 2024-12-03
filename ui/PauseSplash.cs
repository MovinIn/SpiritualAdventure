using Godot;
using System;
using SpiritualAdventure.levels;
using SpiritualAdventure.ui;

public partial class PauseSplash : MarginContainer
{
  private Button restart, menu, next;
  private static PauseSplash singleton;
  private bool buttonsEnabled;
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
    buttonsEnabled = false;
  }

  public static void Display(State state)
  {
    singleton.next.Visible = true;
    singleton.menu.Visible = true;
    singleton.restart.Visible = true;
    singleton.buttonsEnabled = true;

    if (state is State.Paused or State.Failed)
    {
      singleton.next.Visible = false;
    }

    singleton.Visible = true;
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