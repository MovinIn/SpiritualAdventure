using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public class Narrator : Interactable
{
  private bool interacting;
  private Speaker narrator;
  private string name;
#nullable enable
  private System.Action? NotInteracting;

  public Narrator(Speaker narrator,string name="narrator",System.Action? notInteracting=null)
  {
    this.narrator = narrator;
    this.name = name;
    interacting = false;
    NotInteracting = notInteracting;
  }

  public void SetIdentity(Speaker identity,string name)
  {
    narrator = identity;
    this.name = name;
  }
  
  public void Narrate(SpeechLine lines)
  {
    InteractDisplay.UpdateInteractDisplay(narrator.asTexture(),name,lines,this);
  }

  public void Interact()
  {
    interacting = true;
  }

  public bool OptionInteract(string option)
  {
    return true;
  }

  public bool IsInteracting()
  {
    return interacting;
  }

  public void SetNotInteracting()
  {
    interacting = false;
    NotInteracting?.Invoke();
  }
}