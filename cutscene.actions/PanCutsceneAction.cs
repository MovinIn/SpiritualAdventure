using Godot;
using SpiritualAdventure.levels;

namespace SpiritualAdventure.cutscene.actions;

public class PanCutsceneAction:ICutsceneAction
{
  private Vector2 position;
  
  public PanCutsceneAction(Vector2 position)
  {
    this.position = position;
  }
  
  public void Act()
  {
    if (Level.currentCameraMode != Level.CameraMode.Cutscene) return;
    
    Level.cutsceneCamera.Position = position;
    Level.cutsceneCamera.MakeCurrent();
  }
}