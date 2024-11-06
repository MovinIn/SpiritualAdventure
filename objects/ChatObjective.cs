using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class ChatObjective
{
  private Npc npc;
  public Objective objective { get; }

  public ChatObjective(Npc npc,string description,SpeechLine postCompletionFeedback=null,int timeLimit=-1)
  {
    this.npc = npc;
    objective = ObjectiveBuilder.TimedOrElse(description,timeLimit);
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