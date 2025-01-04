using Godot;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class PanCutsceneAction:CutsceneAction
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