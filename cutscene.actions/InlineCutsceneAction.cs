namespace SpiritualAdventure.cutscene.actions;

public class InlineCutsceneAction:ICutsceneAction
{
  public System.Action action;

  public InlineCutsceneAction(System.Action action)
  {
    this.action = action;
  }

  public void Act()
  {
    action?.Invoke();
  }
}