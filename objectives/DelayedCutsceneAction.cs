using System.Threading.Tasks;

namespace SpiritualAdventure.objects;

public abstract class DelayedCutsceneAction:Action,CutsceneAction
{
  
  protected DelayedCutsceneAction(double initialDelay) : base(initialDelay) { }

  protected abstract void ActAfterDelay();
  
  public async void Act()
  {
    await Task.Delay((int)(initialDelay*1000));
    ActAfterDelay();
  }
}