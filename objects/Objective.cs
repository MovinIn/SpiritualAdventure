using System.Collections.Generic;
using Microsoft.Win32;
using SpiritualAdventure.entities;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.objects;

public class Objective
{
  public enum Status
  {
    Start,Completed,Failed,
  }
  
  public delegate void ObjectiveStatusChangeHandler(Status status,Objective objective);
  
  public string description { get; }
  public bool completed { get; private set; }
  public bool hardFail { get; private set; }
  private List<ObjectiveStatusChangeHandler> handlers=new();
  
#nullable enable
  public SpeechLine? postCompletionFeedback { get; set; }

  public Objective(string description,SpeechLine? postCompletionFeedback=null)
  {
    this.description = description;
    this.postCompletionFeedback = postCompletionFeedback;
  }
  
  public virtual void CompletedObjective()
  {
    completed = !hardFail;
    BroadcastAll(Status.Completed);
  }

  public virtual void FailedObjective()
  {
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

  private void BroadcastAll(Status status)
  {
    foreach (var handler in handlers)
    {
      handler(status,this);
    }
  }
}