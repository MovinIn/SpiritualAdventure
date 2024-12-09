using System.Collections.Generic;
using System.Linq;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class ObjectiveDisplayGroup
{
  public List<IHasObjective> objectives { get; private set; }
  private float timeLimit;

  public ObjectiveDisplayGroup(List<IHasObjective> objectives,float timeLimit=-1)
  {
    this.objectives = objectives;
    this.timeLimit = timeLimit;
  }

  public bool IsTimed()
  {
    return timeLimit.CompareTo(-1f)!=0;
  }

  public float GetTimeLimit()
  {
    return timeLimit;
  }

  public bool AllCompleted()
  {
    return objectives.All(io => io.objective.status == Objective.Status.Completed);
  }

  public bool AnyFailed()
  {
    return objectives.Any(io => io.objective.status == Objective.Status.Failed);
  }

}