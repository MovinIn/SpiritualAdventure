using System;
using System.Linq;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class OptionObjective
{
  private Npc npc;
  private string answer;
  private Objective objective;
  private string[] assassinOptions;

  public OptionObjective(Npc npc, string correctOption,Objective objective,string[]incorrectOptions=null)
  {
    this.npc = npc;
    this.objective = objective;
    answer = correctOption;
    npc.SetOptionHandler(OnOption);
    assassinOptions = assassinOptions == null ? Array.Empty<string>() : incorrectOptions;
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