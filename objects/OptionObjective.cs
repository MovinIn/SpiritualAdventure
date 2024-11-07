using System;
using System.Linq;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class OptionObjective
{
  private Npc npc;
  private string answer;
  public Objective objective { get; }
  private string[] assassinOptions;

  public OptionObjective(Npc npc, string correctOption,Objective objective,string[]incorrectOptions=null)
  {
    this.npc = npc;
    this.objective = objective;
    answer = correctOption;
    npc.SetOptionHandler(OnOption);
    assassinOptions = incorrectOptions ?? Array.Empty<string>();
  }

  private bool OnOption(string option)
  {
    if (!npc.OnOption(option))
    {
      return false;
    }

    if (answer.Equals(option))
    {
      objective.CompletedObjective();
    }
    
    if (assassinOptions.Contains(option))
    {
      objective.FailedObjective();
    }

    return true;
  }
}