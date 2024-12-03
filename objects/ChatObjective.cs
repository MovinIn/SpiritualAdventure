using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class ChatObjective: IHasObjective
{
  private Npc npc;
  public Objective objective { get; }

  public ChatObjective(Npc npc,Objective objective,SpeechLine postCompletionFeedback=null)
  {
    this.npc = npc;
    this.objective = objective;
    objective.postCompletionFeedback = postCompletionFeedback;
    npc.SetInteractHandler(OnInteract);
  }

  public void OnInteract()
  {
    if (!objective.completed)
    {
      objective.CompletedObjective();
      if (objective.postCompletionFeedback!=null)
      {
        return;
      }
    }
    npc.OnInteract();
  }
}