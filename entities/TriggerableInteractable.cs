namespace SpiritualAdventure.entities;

public interface TriggerableInteractable:Interactable
{
  public void HideInteractTrigger();
  public void ShowInteractTrigger();
  public string GetInteractTrigger();
}