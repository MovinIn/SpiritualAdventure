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
  private Vector2 cutscenePosition;

  public SimpleCutsceneObjective(List<Tuple<SpeechAction, List<CutsceneAction>>> actions,Vector2 cutscenePosition)
  {
    // Every cutsceneAction (movement, effects, etc.) should be initiated by a speechaction.
    // Therefore, List<SpeechAction,CutsceneAction>
    this.actions = actions;
    this.cutscenePosition = cutscenePosition;
    objective = new Objective("Watch the Cutscene!");
    objective.AddChangeHandler(OnObjectiveStatusChangedHandler);
  }

  private void OnObjectiveStatusChangedHandler(Objective.Status status,Objective objective)
  {
    if (this.objective != objective || status != Objective.Status.Initiated) return;
    
    Level.SetCutscene(true,cutscenePosition);
    
    if (actions.Count == 0)
    {
      Completed();
      return;
    }
    
    actions[0].Item1.Act();
    QueueNextActions();
  }

  private void QueueNextActions()
  {
    actions[actionIndex].Item1.narrator.NotInteracting = ()=>
    {
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
    objective.CompletedObjective();
    Level.SetCutscene(false);
  }
  
  
}