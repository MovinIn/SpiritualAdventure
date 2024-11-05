namespace SpiritualAdventure.objects;

public class ChatObjective
{
  private Npc npc;
  private Objective objective;
  
  public ChatObjective(Npc npc,string description,int timeLimit=-1)
  {
    this.npc = npc;
    objective = ObjectiveBuilder.TimedOrElse(description,timeLimit);
    npc.SetInteractHandler(OnInteract);
  }

  public void OnInteract()
  {
    objective.CompletedObjective();
    npc.OnInteract();
  }
}