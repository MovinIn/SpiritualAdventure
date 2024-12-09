using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;

namespace SpiritualAdventure.ui;

public partial class InteractDisplay : MarginContainer
{

  public enum SpeechType
  {
    Line,Option,Finished
  }
  
  private static InteractDisplay singleton;
  
  private static SpeechDisplay speechDisplay;
  private static SpeakerDetails speakerDetails;
  private static Option[] options;
  private static SpeechLine currentSpeechLine;
  private static Interactable currentInteractable;
  private static double currSpeechDelay;
  private static HashSet<Action<SpeechType, string>> interactHandlers;

  private InteractDisplay()
  {
    singleton = this;
    interactHandlers = new HashSet<Action<SpeechType, string>>();
    options = new Option[4];
    currentSpeechLine = null;
    currentInteractable = null;
    currSpeechDelay = 0;
  }
  
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    for (int i = 1; i < 5; i++) {
      options[i-1]=GetNode<Option>("%Option"+i);
    }

    speechDisplay = GetNode<SpeechDisplay>("%Speech");
    speakerDetails=GetNode<SpeakerDetails>("%SpeakerDetails");
		
    Visible = false;
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
      Exit();
      return;
    }
    currentSpeechLine = speech;
    speechDisplay.SetSpeech(speech.line);
    PingHandlers(SpeechType.Line,speech.line);
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
    PingHandlers(SpeechType.Option,option);
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
    PingHandlers(SpeechType.Finished,"");
    singleton.Visible = false;
    speechDisplay.SetSpeech("");
    currentInteractable?.SetNotInteracting();
    currentInteractable = null;
    currSpeechDelay = 0;
  }

  public static void AttachInteractHandler(Action<SpeechType,string> handler)
  {
    interactHandlers.Add(handler);
  }

  public static void DetachInteractHandler(Action<SpeechType, string> handler)
  {
    interactHandlers.Remove(handler);
  }

  private static void PingHandlers(SpeechType type,string content)
  {
    foreach (Action<SpeechType,string> interactHandler in interactHandlers)
    {
      interactHandler.Invoke(type,content);
    }
  }
}