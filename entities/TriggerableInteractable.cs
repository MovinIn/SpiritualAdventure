namespace SpiritualAdventure.entities;

public interface TriggerableInteractable:Interactable
{
  public string trigger { get; }
  public void HideInteractTrigger();
  public void ShowInteractTrigger();
}