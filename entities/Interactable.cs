namespace SpiritualAdventure.entities;

public interface Interactable
{
    public void Interact();
    public bool OptionInteract(string option);
    public bool IsInteracting();
    public void SetNotInteracting();
    
}