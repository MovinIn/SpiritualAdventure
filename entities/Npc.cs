using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.levels;
using SpiritualAdventure.ui;
using SpiritualAdventure.utility;

//TODO: create entity class for static objects to interact with
namespace SpiritualAdventure.entities;

[JsonObject(MemberSerialization.OptIn)]
public partial class Npc : AnimatableBody2D, IJsonParseable
{
  protected const float SPEED = 120f;
  protected CharacterSprite sprite;
  protected InteractTriggerDisplay interactTrigger;
  protected SpeechLine currLine;
  protected double currSpeechDelay;
  
  protected Speaker speaker;
  protected Queue<SpeechLine> speech;
  protected string name;
  
  
  public static T Instantiate<T>(string path) where T:Npc
  {
    
    return Instantiate().SafelySetScript<T>(path);
  }

  public static Npc Instantiate()
  {
    return ResourceLoader.Load<PackedScene>("res://entities/Npc.tscn").Instantiate<Npc>();
  }
  
  public static Npc Instantiate(Vector2 position)
  {
    var instance = Instantiate();
    instance.Position = position;
    return instance;
  }
  
  public virtual void Parse(JObject json)
  {
    if (json.TryGetValue("position", out var positionToken))
    {
      Position = positionToken.ToObject<Vector2>();
    }
    
    if (json.TryGetValue("name", out var nameToken) && json.TryGetValue("speaker", out var speakerToken))
    {
      Who(speakerToken.ToObject<Speaker>(),nameToken.ToObject<string>());
    }

    if (json.TryGetValue("trigger", out var triggerToken) && json.TryGetValue("content", out var contentToken))
    {
      UseTrigger(triggerToken.ToObject<string>(),contentToken.ToObject<string>());
    }

    if (json.TryGetValue("speech", out var speechToken))
    {
      SetSpeech(speechToken.ToObject<List<SpeechLine>>());
    }
  }

  public Npc()
  {
    speech=new Queue<SpeechLine>();
    InitNodes();
  }

  /**
   * Initializes the children node references if possible. 
   */
  private void InitNodes()
  {
    if (!HasNode("Sprite")) return;
    sprite=GetNode<CharacterSprite>("Sprite");
    interactTrigger = GetNode<InteractTriggerDisplay>("InteractTriggerDisplay");
  }
  
  public override void _Process(double delta)
  {
    if (Level.Paused()) return;
    
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
    interactHandler ??= OnInteract;
    interactTrigger.SetInteractHandler(interactHandler);
  }

  public void SetOptionHandler(Func<string,bool> optionHandler=null)
  {
    optionHandler ??= OnOption;
    interactTrigger.SetOptionHandler(optionHandler);
  }
  
  public void UseTrigger(string trigger,string content,System.Action interactHandler=null)
  {
    interactHandler ??= OnInteract;
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
		
    sprite.updateRotation(Level.player.Position.X-Position.X);
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
    currLine = currLine.options![option];
    return true;
  }
	
  protected void IdleOrElse(string animation="idle"){
    sprite.Play(animation);
  }

  public override void _Notification(int what)
  {
    if (what == NotificationSceneInstantiated)
    {
      
      GD.Print("initializing nodes");
      InitNodes();
    }
  }
}