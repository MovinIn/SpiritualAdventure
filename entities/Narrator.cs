using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public class Narrator : Interactable
{
  [JsonIgnore]
  private bool interacting;
  
  private Speaker narrator;
  private string name;
  
#nullable enable
  [JsonIgnore]
  public System.Action? NotInteracting { get; set; }
  
  private SpeechLine? currLine;
#nullable disable
  
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
    if (currLine!.Finished())
    {
      return null;
    }

    if (!currLine.HasNext())
    {
      return currLine;
    }
    
    currLine = currLine.next!;
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

  public void OptionInteract(string option)
  {
    currLine = currLine?.options![option];
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