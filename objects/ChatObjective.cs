using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class ChatObjective: IHasObjective
{
  private Npc npc;
  public Objective objective { get; }

  public ChatObjective(Npc npc,Objective objective)
  {
    this.npc = npc;
    this.objective = objective;
    npc.SetInteractHandler(OnInteract);
  }

  public void OnInteract()
  {
    if (!objective.completed&&objective.Initiated())
    {
      if (objective.postCompletionFeedback!=null)
      {
        npc.StopInteract();
      }
      
      objective.CompletedObjective();
      
      if (objective.postCompletionFeedback!=null)
      {
        return;
      }
    }
    npc.OnInteract();
  }
}