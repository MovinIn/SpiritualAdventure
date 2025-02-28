#nullable enable

using Newtonsoft.Json;
using SpiritualAdventure.objectives;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public class Narrator : Interactable
{
  [JsonIgnore] public bool isInteracting { get; private set; }

  private Identity identity;
  
  [JsonIgnore]
  public System.Action? NotInteracting { get; set; }
  
  private SpeechLine? currLine;
  
  public Narrator(Speaker speaker=Speaker.Archer,string name="Narrator",System.Action? notInteracting=null)
    :this(new Identity(speaker,name),notInteracting) { }

  public Narrator(Identity identity, System.Action? notInteracting = null)
  {
    this.identity = identity;
    isInteracting = false;
    NotInteracting = notInteracting;
  }

  public void SetIdentity(Speaker speaker,string name)
  {
    identity = new Identity(speaker, name);
  }
  
  public void Narrate(SpeechLine lines)
  {
    isInteracting = true;
    currLine = lines;
    InteractDisplay.UpdateInteractDisplay(identity,lines,this);
  }

  public SpeechLine? NextLine()
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
    SpeechLine? line = NextLine();
    if (line == null)
    {
      InteractDisplay.Exit();
      return;
    }
    
    InteractDisplay.UpdateInteractDisplay(identity,line,this);
  }

  public void OptionInteract(string option)
  {
    currLine = currLine?.options![option];
  }

  public void SetNotInteracting()
  {
    isInteracting = false;
    NotInteracting?.Invoke();
  }
}