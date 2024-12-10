using System.Threading.Tasks;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.objects;

public abstract class DelayedCutsceneAction:Action,CutsceneAction
{
  
  protected DelayedCutsceneAction(double initialDelay) : base(initialDelay) { }

  protected abstract void ActAfterDelay();
  
  public void Act()
  {
    GameTimer.Add(ActAfterDelay,initialDelay);
  }
}