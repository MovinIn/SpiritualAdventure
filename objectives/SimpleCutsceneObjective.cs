using System;
using System.Collections.Generic;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class SimpleCutsceneObjective: IHasObjective
{
  public Objective objective { get; }
  private readonly List<Tuple<SpeechAction, List<ICutsceneAction>>> actionGroups;
  private int actionGroupIndex;

  public SimpleCutsceneObjective(List<Tuple<SpeechAction, List<ICutsceneAction>>> actionGroups)
  {
    // Every cutsceneAction (movement, effects, etc.) should be initiated by a speechaction.
    // Therefore, List<SpeechAction,CutsceneAction>
    this.actionGroups = actionGroups;
    objective = new Objective("Watch the Cutscene!");
    objective.AddChangeHandler(OnObjectiveStatusChangedHandler);
  }

  private void OnObjectiveStatusChangedHandler(Objective.Status status,Objective objective)
  {
    if (this.objective != objective || status != Objective.Status.Initiated) return;
    
    Level.SetCameraMode(Level.CameraMode.Cutscene);
    
    if (actionGroups.Count == 0)
    {
      Completed();
      return;
    }
    
    QueueNextActions();
    actionGroups[0].Item1.Act();
  }

  private void QueueNextActions()
  {
    
    actionGroups[actionGroupIndex].Item1.narrator.NotInteracting = ()=>
    {
      
      if (actionGroups.Count <=actionGroupIndex)
      {
        return;
      }
      
      foreach (var action in actionGroups[actionGroupIndex].Item2)
      {
        action.Act();
      }

      if (actionGroups.Count <= ++actionGroupIndex)
      {
        Completed();
        return;
      }
      
      actionGroups[actionGroupIndex].Item1.Act();
      QueueNextActions();
    };
  }

  private void Completed()
  {
    Level.SetCameraMode(Level.CameraMode.Player);
    objective.CompletedObjective();
  }

  public static Tuple<SpeechAction, List<ICutsceneAction>> DelayedActionGroupWithoutSpeech(float delay,
    List<ICutsceneAction> actionGroup=null)
  {
    actionGroup ??= new List<ICutsceneAction>();
    return new Tuple<SpeechAction, List<ICutsceneAction>>(new SpeechAction(new Narrator(), null, delay), actionGroup);
  }
}