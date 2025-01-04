using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;

namespace SpiritualAdventure.objects;

public class SimpleCutsceneObjective: IHasObjective
{
  public Objective objective { get; }
  private List<Tuple<SpeechAction, List<CutsceneAction>>> actions;
  private int actionIndex;

  public SimpleCutsceneObjective(List<Tuple<SpeechAction, List<CutsceneAction>>> actions)
  {
    // Every cutsceneAction (movement, effects, etc.) should be initiated by a speechaction.
    // Therefore, List<SpeechAction,CutsceneAction>
    this.actions = actions;
    objective = new Objective("Watch the Cutscene!");
    objective.AddChangeHandler(OnObjectiveStatusChangedHandler);
  }

  private void OnObjectiveStatusChangedHandler(Objective.Status status,Objective objective)
  {
    if (this.objective != objective || status != Objective.Status.Initiated) return;
    
    Level.SetCameraMode(Level.CameraMode.Cutscene);
    
    if (actions.Count == 0)
    {
      Completed();
      return;
    }
    
    QueueNextActions();
    actions[0].Item1.Act();
  }

  private void QueueNextActions()
  {
    
    actions[actionIndex].Item1.narrator.NotInteracting = ()=>
    {
      
      if (actions.Count <=actionIndex)
      {
        return;
      }
      
      foreach (var action in actions[actionIndex].Item2)
      {
        action.Act();
      }

      if (actions.Count <= ++actionIndex)
      {
        Completed();
        return;
      }
      
      actions[actionIndex].Item1.Act();
      QueueNextActions();
    };
  }

  private void Completed()
  {
    Level.SetCameraMode(Level.CameraMode.Player);
    objective.CompletedObjective();
  }

  public static Tuple<SpeechAction, List<CutsceneAction>> DelayedActionsWithoutSpeech(float delay,
    List<CutsceneAction> actions=null)
  {
    actions ??= new List<CutsceneAction>();
    return new Tuple<SpeechAction, List<CutsceneAction>>(new SpeechAction(new Narrator(), null, delay), actions);
  }
  
}