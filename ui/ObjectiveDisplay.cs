using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using Action = System.Action;

namespace SpiritualAdventure.ui;

public partial class ObjectiveDisplay : HBoxContainer
{
  private static ObjectiveDisplay instance;
  private static RichTextLabel description;
  private static TimerDisplay timerDisplay;
  private static Action Failed;

#nullable enable
  private static ObjectiveDisplayGroup? objectiveGroup;
#nullable disable

  public ObjectiveDisplay()
  {
    instance = this;
    Failed = null;
    objectiveGroup = null;
  }
  
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    description=GetNode<RichTextLabel>("Description");
    timerDisplay=GetNode<TimerDisplay>("%TimerDisplay");
    timerDisplay.Visible = false;
  }

  public override void _Process(double delta)
  {
    if (Level.Paused())
    {
      RunTimer(false);
      return;
    }
    if (!IsObjectiveRunning()) return;
    
    RunTimer(objectiveGroup!.IsTimed());
  }

  public static bool IsObjectiveRunning()
  {
    return objectiveGroup != null;
  }

  public static bool ContainsObjective(Objective objective)
  {
    return IsObjectiveRunning() && objectiveGroup!.objectives.Any(i => i.objective == objective);
  }
  
  public static void UpdateObjective(ObjectiveDisplayGroup objectiveDisplayGroup, Action onFail)
  {
    Failed = onFail;
    objectiveGroup = objectiveDisplayGroup;
    description.Text = "Objectives";
    objectiveGroup!.objectives.ForEach(io=>description.Text+="\n"+io.objective.description);

    if (objectiveDisplayGroup.IsTimed())
    {
      timerDisplay.Visible = true;
      timerDisplay.Update(objectiveDisplayGroup.GetTimeLimit());
    }
    else
    {
      timerDisplay.Visible = false;
    }
    objectiveDisplayGroup.objectives.ForEach(io=>io.objective.AddChangeHandler(UpdateObjectiveStatus));
  }

  public static void UpdateDescription()
  {
    
    description.Text = "Objectives";

    if (!IsObjectiveRunning()) return;
    
    foreach (var iObjective in objectiveGroup!.objectives)
    {
      switch (iObjective.objective.status)
      {
        case Objective.Status.Completed:
          description.Text += "\n[color=green]Objective Complete![/color]";
          break;
        case Objective.Status.Failed:
          description.Text += "\n[color=red]Objective Failed![/color]";
          break;
        default:
          description.Text += "\n" + iObjective.objective.description;
          break;
      }
    }
  }
  
  public static void UpdateObjectiveStatus(Objective.Status status, Objective objective)
  {
    if (!ContainsObjective(objective)||status==Objective.Status.Initiated) return;
    
    UpdateDescription();

    if (objectiveGroup!.AnyFailed())
    {
      Reset();
      return;
    }
    
    if (objectiveGroup!.AllCompleted())
    {
      Reset();
    }
  }

  private static void Reset()
  {
    objectiveGroup = null;
    Failed = null;
    RunTimer(false);
  }

  public static void RunTimer(bool on)
  {
    timerDisplay.On(on);
  }

  public static void ObjectiveFailed()
  {
    Failed?.Invoke();
  }
  
  public static void OnTimerTimeout()
  {
    ObjectiveFailed();
  }
}