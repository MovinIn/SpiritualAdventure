namespace SpiritualAdventure.objects;

public interface IHasObjective
{
  public Objective objective { get; }

  public bool IsCurrent()
  {
    return !objective.completed && objective.Initiated();
  }

  public void Start() { }
}