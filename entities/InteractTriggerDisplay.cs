using Godot;
using System;
using Godot.Collections;
using SpiritualAdventure.entities;

public partial class InteractTriggerDisplay : RichTextLabel,TriggerableInteractable
{
  public char key { get; private set; }
  public string trigger { get; private set; }
  public string content { get; private set; }
  public bool isInteracting { get; private set; }
  private Action _onInteract;
  private Action<string> _onOption;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    key = 'Q';
    trigger = "interact";
    Visible = false;
    isInteracting = false;
  }

  public void SetInteractHandler(System.Action interactHandler)
  {
    _onInteract = interactHandler;
  }

  public void SetOptionHandler(Action<string> optionHandler)
  {
    _onOption = optionHandler;
  }

  public void SetContent(string trigger, string content)
  {
    this.trigger = trigger;
    Array<InputEvent> input = InputMap.ActionGetEvents(trigger);
    if (input == null || input.Count == 0)
    {
      throw new InvalidOperationException("Trigger Not Found");
    }
    key = input[0].AsText()[0];
    this.content = content;
  }

  public void HideInteractTrigger()
  {
    SetNotInteracting();
    Visible = false;
  }

  public bool HasContent()
  {
    return !String.IsNullOrEmpty(content);
  }
	
  public void ShowInteractTrigger()
  {
    if (!HasContent()) return;

    Text = "[center]"+"[" + key + "] " + content+"[/center]";
    Visible = true;
  }

  public void Interact()
  {
    isInteracting = true;
    _onInteract();
  }

  public void OptionInteract(string option)
  {
    _onOption(option);
  }

  public void SetNotInteracting()
  {
    isInteracting = false;
  }
}