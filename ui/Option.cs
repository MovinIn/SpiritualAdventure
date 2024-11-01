using System;
using Godot;

public partial class Option : MarginContainer
{
    [Signal]
    public delegate void OnOptionPressedEventHandler(string option);
    private RichTextLabel optionText;
    private string option;
    public override void _Ready()
    {
        optionText = GetNode<RichTextLabel>("%OptionText");
    }

    public void SetText(string option)
    {
        this.option = option;
        optionText.Text = "[center]"+option+"[/center]";
    }

    public bool HasText()
    {
        return !string.IsNullOrWhiteSpace(option);
    }

    public void OnButtonPressed()
    {
        EmitSignal(SignalName.OnOptionPressed, option);
    }
}
