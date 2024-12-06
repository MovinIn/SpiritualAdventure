namespace SpiritualAdventure.objects;

public static class ObjectiveBuilder
{
  public static Objective TimedOrElse(string description,int timeLimit=-1)
  {
    return timeLimit != -1 ? new SimpleTimedObjective(description,timeLimit) 
      : new Objective(description);
  }
}