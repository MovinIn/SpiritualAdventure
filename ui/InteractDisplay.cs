using System.Linq;
using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;

namespace SpiritualAdventure.ui;

public partial class InteractDisplay : MarginContainer
{
  private static InteractDisplay singleton;
  
  private static SpeechDisplay speechDisplay;
  private static SpeakerDetails speakerDetails;
  private static Option[] options;
  private static SpeechLine currentSpeechLine;
  private static Interactable currentInteractable;
  private static double currSpeechDelay;
	
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    options = new Option[4];
    for (int i = 1; i < 5; i++) {
      options[i-1]=GetNode<Option>("%Option"+i);
    }

    speechDisplay = GetNode<SpeechDisplay>("%Speech");
    speakerDetails=GetNode<SpeakerDetails>("%SpeakerDetails");
		
    Visible = false;
    singleton = this;
  }

  public static void UpdateInteractDisplay(Texture2D texture, string name, SpeechLine speech, Interactable interactable)
  {
    if (currentInteractable != interactable)
    {
      currentInteractable?.SetNotInteracting();
    }

    currentInteractable = interactable;
    speakerDetails.setSpeaker(texture, name);
    UpdateSpeechLine(speech);
  }

  public override void _Process(double delta)
  {
    if (!IsActive()) return;
    currSpeechDelay += delta;
  }
  
  public override void _Input(InputEvent @event)  
  {

    if (Level.Paused())
    {
      return;
    }
    
    if (!IsActive())
    {
      InteractProximityFilter.OnInput(@event);
      return;
    }

    if (@event.IsActionPressed("interact"))
    {
      if (currentSpeechLine!=null&&currSpeechDelay<currentSpeechLine.GetDelay()) return;

      currSpeechDelay = 0;
      
      if (!currentInteractable.IsInteracting())
      {
        Exit();
        return;
      }
      
      currentInteractable.Interact();
    }
  }
  
  

  private static void UpdateSpeechLine(SpeechLine speech)
  {
    if (speech == null)
    {
      currentInteractable?.SetNotInteracting();
      currentInteractable = null;
      return;
    }
    currentSpeechLine = speech;
    InteractDisplay.speechDisplay.SetSpeech(speech.line);
    singleton.Visible = true;
    HideOptions();
        
    foreach (var option in options)
    {
      option.SetText("");
    }
    for (int i = 0; i < options.Length && i < speech.options?.Count; i++)
    {
      options[i].SetText(speech.options.Keys.ElementAt(i));
    }
  }

  public void SpeechDisplayFinishedUpdating()
  {
    ShowOptions();
  }

  public void OnOptionPressed(string option)
  {
    currentInteractable.OptionInteract(option);
    UpdateSpeechLine(currentSpeechLine?.options![option]);
  }

  private static void HideOptions()
  {
    foreach (var option in options)
    {
      option.SetVisible(false);
    }
  }
    
  private static void ShowOptions()
  {
    foreach (var option in options)
    {
      option.SetVisible(option.HasText());
    }
  }

  public static bool SpeechContentFinished()
  {
    return speechDisplay.finished;
  }

  public static bool IsActive()
  {
    return currentInteractable != null;
  }

  public static void Exit()
  {
    singleton.Visible = false;
    speechDisplay.SetSpeech("");
    currentInteractable?.SetNotInteracting();
    currentInteractable = null;
    currSpeechDelay = 0;
  }
}