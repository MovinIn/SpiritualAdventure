using System;
using System.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class OptionObjective : IHasObjective
{
  private Npc npc;
  private string answer;
  private string[] assassinOptions;

  public Objective objective { get; }
  
  public OptionObjective(Npc npc, string correctOption,Objective objective,string[]incorrectOptions=null)
  {
    this.npc = npc;
    this.objective = objective;
    answer = correctOption;
    assassinOptions = incorrectOptions ?? Array.Empty<string>();
  }

  public void Start()
  {
    npc.SetOptionHandler(OnOption);
  }

  private void OnOption(string option)
  {
    if (answer.Equals(option))
    {
      objective.CompletedObjective();
    }
    
    if (assassinOptions.Contains(option))
    {
      objective.FailedObjective();
    }

    npc.OnOption(option);
  }
}