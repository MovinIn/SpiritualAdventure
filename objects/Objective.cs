using System.Collections.Generic;
using Godot;
using Microsoft.Win32;
using Newtonsoft.Json;
using SpiritualAdventure.entities;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.objects;

public class Objective
{
  public enum Status
  {
    Uninitiated,Start,Completed,Failed
  }
  
  public delegate void ObjectiveStatusChangeHandler(Status status,Objective objective);
  
  [JsonIgnore]
  public bool completed { get; private set; }
  [JsonIgnore]
  public bool hardFail { get; private set; }
  [JsonIgnore]
  private List<ObjectiveStatusChangeHandler> handlers=new();
  
  [JsonProperty("postCompletionFeedback", NullValueHandling=NullValueHandling.Ignore)]
#nullable enable
  public SpeechLine? postCompletionFeedback { get; set; }

  public string description { get; }

  public Status status { get; private set; }


  [JsonConstructor]
  public Objective(string description,SpeechLine? postCompletionFeedback=null)
  {
    this.description = description;
    this.postCompletionFeedback = postCompletionFeedback;
    status = Status.Uninitiated;
  }
  
  public virtual void CompletedObjective()
  {
    if (!Initiated()) return;
    
    if (completed)
    {
      return;
    }
    
    completed = !hardFail;
    BroadcastAll(Status.Completed);
  }

  public virtual void FailedObjective()
  {
    if (!Initiated()) return;
    
    hardFail = true;
    completed = false;
    BroadcastAll(Status.Failed);
  }
  
  /**
   * returns -1 if the objective is not time constrained.
   */
  public virtual int GetTimeLimit()
  {
    return -1;
  }

  public virtual bool IsTimeRunning()
  {
    return false;
  }

  public virtual bool IsTimeConstrained()
  {
    return GetTimeLimit() != -1;
  }
  
  public virtual void SetAsObjective()
  {
    ObjectiveDisplay.instance.UpdateObjective(this,FailedObjective);
    BroadcastAll(Status.Start);
  }
  
  public void AddChangeHandler(ObjectiveStatusChangeHandler handler)
  {
    handlers.Add(handler);
  }

  public void RemoveChangeHandler(ObjectiveStatusChangeHandler handler)
  {
    handlers.Remove(handler);
  }

  private void BroadcastAll(Status status)
  {
    this.status = status;
    foreach (var handler in handlers)
    {
      handler(status,this);
    }
  }

  public bool Initiated()
  {
    return status != Status.Uninitiated;
  }

}