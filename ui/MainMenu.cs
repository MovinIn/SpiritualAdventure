using Godot;
using System;
using SpiritualAdventure.ui;

public partial class MainMenu : CanvasLayer
{
  public void PlayPressed()
  {
    Root.LevelSelect();
  }
}
