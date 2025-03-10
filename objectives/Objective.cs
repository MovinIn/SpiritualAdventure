﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.objectives;

public class Objective:ICloneable<Objective>
{
  public enum Status
  {
    Uninitiated,Initiated,Completed,Failed
  }
  
  public bool completed { get; private set; }
  public bool hardFail { get; private set; }
  private readonly List<Action<Status,Objective>> handlers=new();
  
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
    if (!Initiated() || hardFail|| completed) return;

    completed = true;
    BroadcastAll(Status.Completed);
  }

  public virtual void FailedObjective()
  {
    if (!Initiated()||completed) return;
    
    hardFail = true;
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
    BroadcastAll(Status.Initiated);
  }
  
  public void AddChangeHandler(Action<Status,Objective> handler)
  {
    handlers.Add(handler);
  }

  public void RemoveChangeHandler(Action<Status,Objective> handler)
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
  
  /**
 * Returns true if not completed, not failed, and initiated. Returns false otherwise.
 */
  public bool IsActive()
  {
    return !completed && !hardFail && Initiated();
  }

  public Objective Clone()
  {
    return new Objective(description, postCompletionFeedback);
  }
}