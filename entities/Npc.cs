using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.ui;

//TODO: create entity class for static objects to interact with
namespace SpiritualAdventure.entities;

public partial class Npc : AnimatableBody2D
{
  protected const float SPEED = 120f;
  protected CharacterSprite sprite;
  protected Speaker speaker;
  protected Queue<SpeechLine> speech;
  protected InteractTriggerDisplay interactTrigger;
  protected string name;
  protected SpeechLine currLine;
  protected double currSpeechDelay;
  
  public static Npc Instantiate(Vector2 position)
  {
    var instance=ResourceLoader.Load<PackedScene>("res://entities/Npc.tscn").Instantiate<Npc>();
    instance.Position = position;
    return instance;
  }
  
  protected Npc()
  {
    speech=new Queue<SpeechLine>();
  }
	
  public override void _Ready()
  {
    sprite=GetNode<CharacterSprite>("Sprite");
    interactTrigger = GetNode<InteractTriggerDisplay>("InteractTriggerDisplay");
  }

  public override void _Process(double delta)
  {
    currSpeechDelay += delta;
  }

  public void Who(Speaker speaker,string name)
  {
    this.speaker = speaker;
    this.name = name;
    sprite.SetSpriteFrames(speaker.asFrames());
  }

  public void SetInteractHandler(System.Action interactHandler=null)
  {
    if (interactHandler is null)
    {
      interactHandler = OnInteract;
    }
    interactTrigger.SetInteractHandler(interactHandler);
  }

  public void SetOptionHandler(Func<string,bool> optionHandler=null)
  {
    if (optionHandler is null)
    {
      optionHandler = OnOption;
    }
    interactTrigger.SetOptionHandler(optionHandler);
  }
  
  public void UseTrigger(string trigger,string content,System.Action interactHandler=null)
  {
    if (interactHandler is null)
    {
      interactHandler = OnInteract;
    }
    interactTrigger.SetContent(trigger,content);
    SetInteractHandler(interactHandler);
    interactTrigger.SetOptionHandler(OnOption);
  }
	
  public void SetSpeech(List<SpeechLine> speech)
  {
    if (!interactTrigger.HasContent())
      throw new InvalidOperationException("Trigger does not have content. Please call UseTrigger() first.");
		
    this.speech.Clear();
    foreach (var se in speech)
      this.speech.Enqueue(se);
  }

  public SpeechLine NextLine()
  {
    if (speech.Count == 0) return null;
    if (currLine==null)
    {
      currLine = speech.Peek();
      return currLine;
    }
    if (currLine.Finished())
    {
      StopInteract();
      speech.Enqueue(speech.Dequeue());
      return null;
    }
    if (!currLine.HasNext())
      return currLine;
		
    currLine = currLine.next;
    return currLine;
  }

  public void OnInteractBodyEntered(Node2D body)
  {
    if (interactTrigger.HasContent() && body is Player)
    {
      InteractProximityFilter.Add(interactTrigger,Position);
    }
  }

  protected void StopInteract()
  {
    if (interactTrigger.IsInteracting()) {
      InteractDisplay.Exit();
    }
    currLine = null;
  }
	
  public void DetachInteract()
  {
    StopInteract();
    InteractProximityFilter.Remove(interactTrigger);
  }
	
  public void OnInteractBodyExited(Node2D body)
  {
    if (body is Player)
    {
      DetachInteract();
    }
  }

  private bool OnAnyInteractAction()
  {
    if (currLine!=null&&currSpeechDelay<currLine.GetDelay()) return false;
		
    sprite.updateRotation(Root.player.Position.X-Position.X);
    currSpeechDelay = 0;
    return true;
  }
	
  public void OnInteract()
  {
    if (!OnAnyInteractAction()) return;
		
    SpeechLine line = NextLine();
    if (line == null)
    {
      interactTrigger.SetNotInteracting();
      return;
    }

    InteractDisplay.UpdateInteractDisplay(speaker.asTexture(),name,line,interactTrigger);
  }

  public bool OnOption(string option)
  {
    if (!OnAnyInteractAction()) return false;
    currLine = currLine.options[option];
    return true;
  }
	
  protected void IdleOrElse(string animation="idle"){
    sprite.Play(animation);
  }

}