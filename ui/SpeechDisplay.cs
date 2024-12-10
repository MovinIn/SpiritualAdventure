using Godot;
using SpiritualAdventure.levels;

public partial class SpeechDisplay : RichTextLabel
{
    [Signal]
    public delegate void SpeechDisplayFinishedUpdatingEventHandler();
    
    public static readonly double letterDelay = 0.02d;
    private double currDelay;
    private int letterIndex;
    public bool finished;
    private string currText;
    private string speech;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currDelay = 0;
        letterIndex = 0;
        currText = "";
        finished = true;
        speech = "";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
      if (Level.Paused()) return;

        if (finished) return;
        currDelay += delta;
        if (currDelay < letterDelay) return;

        while (currDelay >= letterDelay)
        {
            if (letterIndex>=speech.Length) {
                finished = true;
                letterIndex = 0;
                EmitSignal(SignalName.SpeechDisplayFinishedUpdating);
                return;
            }
            currText += speech[letterIndex];
            letterIndex++;
            currDelay -= letterDelay;
        }

        Text = currText;
    }

    public void SetSpeech(string speech)
    {
        this.speech = speech;
        currText = "";
        letterIndex = 0;
        finished = false;
    }
}