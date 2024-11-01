using System.Linq;
using Godot;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.ui;

public partial class InteractDisplay : MarginContainer
{
    private static InteractDisplay singleton;
    private SpeechDisplay speech;
    private SpeakerDetails speakerDetails;
    private Option[] options;
    private SpeechLine currentSpeechLine;
    private Interactable currentInteractable;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        options = new Option[4];
        for (int i = 1; i < 5; i++) {
            options[i-1]=GetNode<Option>("%Option"+i);
        }

        speech = GetNode<SpeechDisplay>("%Speech");
        speakerDetails=GetNode<SpeakerDetails>("%SpeakerDetails");
		
        Visible = false;
        singleton = this;
    }

    public static void UpdateInteractDisplay(Texture2D texture,string name,SpeechLine speech,Interactable interactable)
    {
        singleton.currentInteractable = interactable;
        singleton.speakerDetails.setSpeaker(texture,name);
        UpdateSpeechLine(speech);
    }

    private static void UpdateSpeechLine(SpeechLine speech)
    {
        if (speech == null)
        {
            singleton.currentInteractable?.SetNotInteracting();
            singleton.currentInteractable = null;
            return;
        }
        singleton.currentSpeechLine = speech;
        singleton.speech.SetSpeech(speech.line);
        singleton.Visible = true;
        HideOptions();
        
        foreach (var option in singleton.options)
        {
            option.SetText("");
        }
        for (int i = 0; i < singleton.options.Length && i < speech.options.Count; i++)
        {
            singleton.options[i].SetText(speech.options.Keys.ElementAt(i));
        }
    }

    public void SpeechDisplayFinishedUpdating()
    {
        ShowOptions();
    }

    public void OnOptionPressed(string option)
    {
        GD.Print("inside interact display option pressed");
        if (currentInteractable.OptionInteract(option))
        {
            UpdateSpeechLine(currentSpeechLine?.options[option]);
        }
    }

    private static void HideOptions()
    {
        foreach (var option in singleton.options)
        {
            option.SetVisible(false);
        }
    }
    
    private static void ShowOptions()
    {
        foreach (var option in singleton.options)
        {
            option.SetVisible(option.HasText());
        }
    }

    public static bool SpeechFinished()
    {
        return singleton.speech.finished;
    }

    public static void Exit()
    {
        singleton.Visible = false;
        singleton.speech.SetSpeech("");
        singleton.currentInteractable?.SetNotInteracting();
    }
}