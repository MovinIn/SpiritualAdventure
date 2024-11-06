using System;
using Godot;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.ui;

public partial class ObjectiveDisplay : HBoxContainer
{
  public static ObjectiveDisplay instance { get; private set; }
  private RichTextLabel description;
  private TimerDisplay timerDisplay;
  private Action Failed;

#nullable enable
  private Objective? objective;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    instance = this;
    description=GetNode<RichTextLabel>("Description");
    timerDisplay=GetNode<TimerDisplay>("TimerDisplay");
  }

  public override void _Process(double delta)
  {
    if (!IsObjectiveRunning()) return;
    if (objective!.completed)
    {
      MarkCompleted();
      return;
    }

    RunTimer(objective.IsTimeRunning());
    
  }

  public bool IsObjectiveRunning()
  {
    return objective != null;
  }

  public bool EqualsThisObjective(Objective objective)
  {
    return this.objective == objective;
  }
  
  public void UpdateObjective(Objective objective, Action onFail)
  {
    Failed = onFail;
    this.objective = objective;
    description.Modulate = Colors.White;
    description.Text = "Objective: "+objective.description;
    
    if (objective.IsTimeConstrained())
    {
      timerDisplay.Visible = true;
      timerDisplay.Update(objective.GetTimeLimit());
    }
    else
    {
      timerDisplay.Visible = false;
    }
  }

  public void MarkCompleted()
  {
    description.Text = "Objective: COMPLETE!";
    description.Modulate = Colors.Green;
    objective = null;
    Failed = null;
    RunTimer(false);
  }

  public void RunTimer(bool on)
  {
    timerDisplay.On(on);
  }

  public void SetObjectiveFailed()
  {
    Failed?.Invoke();
  }
  
  public void OnTimerTimeout()
  {
    SetObjectiveFailed();
  }
}