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
    if (this.objective != objective || status != Objective.Status.Start) return;
    
    Level.SetCutscene(true,new Vector2(1000,1000));
    
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