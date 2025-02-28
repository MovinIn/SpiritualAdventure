using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.cutscene.actions;

public abstract class DelayedCutsceneAction:Action,ICutsceneAction
{
  
  protected DelayedCutsceneAction(double initialDelay) : base(initialDelay) { }

  protected abstract void ActAfterDelay();
  
  public void Act()
  {
    GameTimer.Add(ActAfterDelay,initialDelay);
  }
}