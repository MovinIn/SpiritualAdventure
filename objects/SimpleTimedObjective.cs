using Godot;

namespace SpiritualAdventure.objects;

public class SimpleTimedObjective : Objective
{
  public int timeLimit;
  public bool isTimeRunning;

  public SimpleTimedObjective(string description,int timeLimit,bool isTimeRunning=false) :base(description)
  {
    this.timeLimit = timeLimit;
    this.isTimeRunning = isTimeRunning;
  }
  public override int GetTimeLimit()
  {
    return timeLimit;
  }

  public override bool IsTimeRunning()
  {
    return isTimeRunning;
  }

  public override void SetAsObjective()
  {
    isTimeRunning = true;
    base.SetAsObjective();
  }
  
  public override void CompletedObjective()
  {
    isTimeRunning = false;
    base.CompletedObjective();
  }

  public override void FailedObjective()
  {
    isTimeRunning = false;
    base.FailedObjective();
  }
}