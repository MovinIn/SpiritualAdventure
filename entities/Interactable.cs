namespace SpiritualAdventure.entities;

public interface Interactable
{
    public void Interact();
    public void OptionInteract(string option);
    public bool IsInteracting();
    public void SetNotInteracting();
    
}