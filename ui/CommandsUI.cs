using Godot;

namespace SpiritualAdventure.ui;

public partial class CommandsUI : CanvasLayer
{
  private LineEdit commandTerminal;
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    commandTerminal=GetNode<LineEdit>("%CommandTerminal");
    commandTerminal.Visible = false;
  }

  public void FocusCommandTerminal()
  {
    commandTerminal.Visible = true;
    commandTerminal.GrabFocus();
  }

  public void UnFocusCommandTerminal()
  {
    commandTerminal.ReleaseFocus();
  }
}
