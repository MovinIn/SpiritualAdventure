namespace SpiritualAdventure.objectives;

public interface IHasObjective
{
  public Objective objective { get; }

  public void Start() { }
}