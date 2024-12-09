using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.objectives;

public class TargetSpeechObjective:IHasObjective
{
  public Objective objective { get; }
  
  private readonly string targetLine;
  private bool foundTargetLine;

  public TargetSpeechObjective(Objective objective, string targetLine)
  {
    this.objective = objective;
    this.targetLine = targetLine;
    foundTargetLine = false;
    InteractDisplay.AttachInteractHandler(OnInteract);
  }

  private void OnInteract(InteractDisplay.SpeechType type,string line)
  {
    if (objective.IsActive()&&foundTargetLine)
    {
      objective.CompletedObjective();
      return;
    }

    if (type==InteractDisplay.SpeechType.Line && targetLine.Equals(line))
    {
      foundTargetLine = true;
    }
  }
}