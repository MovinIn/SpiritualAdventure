using Godot;
using System;

public partial class Speech : RichTextLabel
{
    private double letterDelay = 0.1;
    private double currDelay;
    private string[] words;
    private int wordIndex;
    private int letterIndex;
    private bool finished;
    private string currText;
    
    private const int wordsPerParagraph = 20;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currDelay = 0;
        wordIndex = 0;
        letterIndex = 0;
        currText = "";
    }

    public void setFinished(bool finished)
    {
        this.finished = finished;
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (finished) return;
        currDelay += delta;
        if (currDelay < letterDelay) return;

        while (currDelay >= letterDelay)
        {
            if (wordIndex==words.Length||(wordIndex != 0 && wordIndex % wordsPerParagraph == 0)) {
                finished = true;
                letterIndex = 0;
                return;
            }

            if (letterIndex == words[wordIndex].Length)
            {
                letterIndex = 0;
                wordIndex++;
                currText += " ";
            }
            else
            {
                currText += words[wordIndex][letterIndex];
                letterIndex++;
            }
            currDelay -= letterDelay;
        }

        Text = currText;
    }

    public void setSpeech(string speech)
    {
        words = speech.Split(" ");
        wordIndex = 0;
        letterIndex = 0;
        finished = false;
    }
}