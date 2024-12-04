using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public class Narrator : Interactable
{
  [JsonIgnore]
  private bool interacting;
  [JsonIgnore]
#nullable enable
  public System.Action? NotInteracting { get; set; }
  
  private Speaker narrator;
  private string name;

  private SpeechLine currLine;

  public Narrator(Speaker narrator=Speaker.Archer,string name="narrator",System.Action? notInteracting=null)
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
    interacting = true;
    currLine = lines;
    InteractDisplay.UpdateInteractDisplay(narrator.asTexture(),name,lines,this);
  }

  public SpeechLine NextLine()
  {
    if (currLine.Finished())
    {
      return null;
    }
    
    currLine = currLine.next;
    return currLine;
  }
  
  
  public void Interact()
  {
    SpeechLine line = NextLine();
    if (line == null)
    {
      InteractDisplay.Exit();
      return;
    }
    
    InteractDisplay.UpdateInteractDisplay(narrator.asTexture(),name,line,this);
  }

  public bool OptionInteract(string option)
  {
    currLine = currLine.options![option];
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