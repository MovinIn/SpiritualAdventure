using Godot;
using SpiritualAdventure.ui;

public partial class Commands : LineEdit
{
  public void OnFocusEnter()
  {
	GD.Print("Focus Entered!");
  }

  public void OnFocusExit()
  {
    Clear();
    Visible = false;
    
  }

  public void OnGuiInput(InputEvent @event)
  {
    if (@event is not InputEventKey eventKey) return;
    
    if (eventKey.Keycode == Key.Enter)
    {
      CommandParser.Parse(Text);
      ReleaseFocus();
    }
  }
}
