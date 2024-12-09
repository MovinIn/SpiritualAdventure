namespace SpiritualAdventure.objects;

public interface IHasObjective
{
  public Objective objective { get; }

  public void Start() { }
}