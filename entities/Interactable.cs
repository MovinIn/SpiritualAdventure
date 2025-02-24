namespace SpiritualAdventure.entities;

public interface Interactable
{
  public bool isInteracting { get; }
  public void Interact();
  public void OptionInteract(string option);
  public void SetNotInteracting();
}