using System.Collections.Generic;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class NegativeObjective:IHasObjective
{
  public Objective objective { get; }

  public NegativeObjective(Objective objective,List<Objective> negativeObjectives)
  {
    this.objective = objective;
    foreach (var negativeObjective in negativeObjectives)
    {
      negativeObjective.AddChangeHandler((status,_) =>
      {
        if (status == Objective.Status.Completed)
        {
          objective.FailedObjective();
        }
      });
    }
    
    objective.AddChangeHandler((status, _) =>
    {
      if (status == Objective.Status.Initiated)
      {
        negativeObjectives.ForEach(o=>o.SetAsObjective()); 
      }
    });
  }
}