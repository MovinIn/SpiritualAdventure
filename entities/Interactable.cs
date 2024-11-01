namespace SpiritualAdventure.entities;

public interface Interactable
{
    public void HideInteractTrigger();
    public void ShowInteractTrigger();
    public void Interact();
    public bool OptionInteract(string option);
    public bool IsInteracting();
    public string GetInteractTrigger();
    public void SetNotInteracting();
    
}