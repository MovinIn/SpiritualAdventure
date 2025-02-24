namespace SpiritualAdventure.entities;

public interface ICloneable<out T>
{
  public T Clone();
}