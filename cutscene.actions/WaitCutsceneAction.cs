namespace SpiritualAdventure.cutscene.actions;

public class WaitCutsceneAction:DelayedCutsceneAction
{
  public WaitCutsceneAction(double initialDelay) : base(initialDelay) { }

  //Do nothing
  protected override void ActAfterDelay() { }
}