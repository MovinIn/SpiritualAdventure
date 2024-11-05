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

  public override bool IsTimeConstrained()
  {
    return true;
  }
}