using System.Collections.Generic;
using System.Linq;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.objectives;

public class ObjectiveDisplayGroup
{
  public class Builder
  {
    private readonly List<IHasObjective> requiredObjectives;
    private readonly List<IHasObjective> hiddenObjectives = new();
    private float timeLimit;

    private Builder(List<IHasObjective> requiredObjectives)
    {
      this.requiredObjectives=requiredObjectives;
    }
    
    public static Builder Init(List<IHasObjective> requiredObjectives)
    {
      return new Builder(requiredObjectives);
    }

    public Builder AddRequiredObjective(IHasObjective objective)
    {
      requiredObjectives.Add(objective);
      return this;
    }

    public Builder AddRequiredObjectives(IEnumerable<IHasObjective> objectives)
    {
      requiredObjectives.AddRange(objectives);
      return this;
    }

    public Builder AddHiddenObjective(IHasObjective objective)
    {
      hiddenObjectives.Add(objective);
      return this;
    }

    public Builder AddHiddenObjectives(IEnumerable<IHasObjective> objectives)
    {
      hiddenObjectives.AddRange(objectives);
      return this;
    }

    public Builder WithTimeLimit(float timeLimit)
    {
      this.timeLimit = timeLimit;
      return this;
    }

    public ObjectiveDisplayGroup Build()
    {
      // Defensive copy so builder lists can still be modified afterwards
      return new ObjectiveDisplayGroup(
        new List<IHasObjective>(requiredObjectives),
        new List<IHasObjective>(hiddenObjectives),
        timeLimit
      );
    }
  }
  
  public List<IHasObjective> requiredObjectives { get; }
  public List<IHasObjective> hiddenObjectives { get; }
  
  public float timeLimit { get; }

  private ObjectiveDisplayGroup(List<IHasObjective> requiredObjectives,
    List<IHasObjective> hiddenObjectives,float timeLimit)
  {
    this.hiddenObjectives = hiddenObjectives;
    this.requiredObjectives = requiredObjectives;
    this.timeLimit = timeLimit;
  }

  public bool IsTimed()
  {
    return timeLimit>0;
  }

  public bool AllCompleted()
  {
    return requiredObjectives.All(io => io.objective.status == Objective.Status.Completed || io is NegativeObjective);
  }

  public bool AnyFailed()
  {
    return requiredObjectives.Any(io => io.objective.status == Objective.Status.Failed);
  }
}