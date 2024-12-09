using System;
using System.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.objectives;

public class OptionObjective : IHasObjective
{
  private string answer;
  private string[] assassinOptions;

  public Objective objective { get; }
  
  public OptionObjective(string correctOption,Objective objective,string[]incorrectOptions=null)
  {
    this.objective = objective;
    answer = correctOption;
    assassinOptions = incorrectOptions ?? Array.Empty<string>();
    InteractDisplay.AttachInteractHandler(OnInteract);
  }

  private void OnInteract(InteractDisplay.SpeechType type,string option)
  {
    if (!objective.Initiated()||type != InteractDisplay.SpeechType.Option) return;
    
    if (answer.Equals(option))
    {
      objective.CompletedObjective();
      InteractDisplay.DetachInteractHandler(OnInteract);
    }
    
    if (assassinOptions.Contains(option))
    {
      objective.FailedObjective();
      InteractDisplay.DetachInteractHandler(OnInteract);
    }
  }
}