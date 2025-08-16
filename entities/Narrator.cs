#nullable enable

using Newtonsoft.Json;
using SpiritualAdventure.objectives;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public class Narrator : Interactable
{
  [JsonIgnore] public bool isInteracting { get; private set; }

  [JsonIgnore]
  public System.Action? NotInteracting { get; set; }
  
  private SpeechLine? currLine;
  
  public static readonly Identity Identity = new(Speaker.Archer, "Narrator");
  
  public Narrator(System.Action? notInteracting = null)
  {
    isInteracting = false;
    NotInteracting = notInteracting;
  }

  public void Narrate(SpeechLine? lines)
  {
    if (lines == null)
    {
      SetNotInteracting();
      return;
    }
    
    isInteracting = true;
    currLine = lines;
    InteractDisplay.UpdateInteractDisplay(lines,this);
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
    
    InteractDisplay.UpdateInteractDisplay(line,this);
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