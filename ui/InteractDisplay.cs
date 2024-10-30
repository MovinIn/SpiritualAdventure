using Godot;
using System;

public partial class InteractDisplay : ColorRect
{
    private Speech speech;
    private SpeakerDetails speakerDetails;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        speech = GetNode<Speech>("MarginSpeech/Speech");
        speakerDetails=GetNode<SpeakerDetails>("MarginSpeaker/SpeakerDetails");
    }

    public void updateInteractDisplay(Texture2D texture,string name,string speech)
    {
        speakerDetails.setSpeaker(texture,name);
        this.speech.setSpeech(speech);
    }
}
