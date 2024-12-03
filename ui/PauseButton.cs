using Godot;
using System;

public partial class PauseButton : Button
{
  public void OnPressed()
  {
	PauseSplash.Display(PauseSplash.State.Paused);
  }
}
