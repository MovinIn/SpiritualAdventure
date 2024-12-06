using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class InlineCutsceneAction:CutsceneAction
{
  private System.Action action;

  public InlineCutsceneAction(System.Action action)
  {
    this.action = action;
  }

  public void Act()
  {
    action?.Invoke();
  }
}