using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.levels;
using SpiritualAdventure.ui;
using SpiritualAdventure.utility;

//TODO: create entity class for static objects to interact with
namespace SpiritualAdventure.entities;

[JsonObject(MemberSerialization.OptIn)]
public partial class Npc : AnimatableBody2D, ICloneable<Npc>
{
  protected const float SPEED = 120f;
  protected CharacterSprite sprite;
  protected InteractTriggerDisplay interactTrigger;
  protected SpeechLine currLine;
  
  protected Speaker speaker;
  protected Queue<SpeechLine> speech;
  protected string name;

#nullable enable
  public Action? FinishedSpeech { private get; set; }
  public Action<string>? NewSpeechLine { private get; set; }
#nullable disable
  
  
  protected static T Instantiate<T>(string path) where T:Npc
  {
    return Instantiate().SafelySetScript<T>(path);
  }
  
  public static Npc Instantiate()
  {
    return ResourceLoader.Load<PackedScene>("res://entities/Npc.tscn").Instantiate<Npc>();
  }

  public Npc WithPosition(Vector2 position)
  {
    Position = position;
    return this;
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
  
  public override void _PhysicsProcess(double delta)
  {
    if (Level.Paused()) return;
    IdleOrElse();
  }

  public void Who(Speaker speaker,string name)
  {
    this.speaker = speaker;
    this.name = name;
    sprite.SetSpriteFrames(speaker.asFrames());
  }

  public void SetInteractHandler(Action interactHandler=null)
  {
    interactHandler ??= OnInteract;
    interactTrigger.SetInteractHandler(interactHandler);
  }

  public void SetOptionHandler(Action<string> optionHandler=null)
  {
    optionHandler ??= OnOption;
    interactTrigger.SetOptionHandler(optionHandler);
  }
  
  public void UseTrigger(string trigger,string content,Action interactHandler=null)
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

  public void StopInteract()
  {
    if (interactTrigger.isInteracting) {
      interactTrigger.SetNotInteracting();
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

  private void UpdateRotation()
  {
    if (Level.currentCameraMode!=Level.CameraMode.Cutscene)
    {
      SetDirection(Level.player.Position.X-Position.X);
    }
  }

  public void SetDirection(float direction)
  {
    sprite.updateRotation(direction);
  }
	
  public void OnInteract()
  {
    UpdateRotation();
	
    SpeechLine line = NextLine();
    if (line == null)
    {
      interactTrigger.SetNotInteracting();
	  
      FinishedSpeech?.Invoke();
	  
      return;
    }
	
    NewSpeechLine?.Invoke(line.line);
	
    InteractDisplay.UpdateInteractDisplay(speaker.asTexture(),name,line,interactTrigger);
  }

  public void OnOption(string option)
  {
    UpdateRotation();
    currLine = currLine.options![option];
	
    if (currLine != null)
    {
      NewSpeechLine?.Invoke(currLine.line);
    }
  }
	
  protected void IdleOrElse(string animation="idle")
  {
    if (sprite.SpriteFrames.HasAnimation(animation))
    {
      sprite.Play(animation);
    }
  }

  public override void _Notification(int what)
  {
    if (what == NotificationSceneInstantiated)
    {
      InitNodes();
    }
  }

  public Npc Clone()
  {
    Npc clone=Instantiate();
    clone.Position = Position;
    clone.Who(speaker,name);
    if (interactTrigger.HasContent())
    {
      clone.UseTrigger(interactTrigger.trigger,interactTrigger.content);
    }
    SetSpeech(clone.speech.ToList());
    
    return clone;
  }
}