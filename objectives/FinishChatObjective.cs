using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

//TODO: maybe somehow use interactdisplay handler? but we don't know which speech finishes...
public class FinishChatObjective:IHasObjective
{
  public Objective objective { get; }
  private Npc npc;

  public FinishChatObjective(Npc npc,Objective objective)
  {
    this.npc = npc;
    this.objective = objective;
  }
    
  public void Start()
  {
    npc.FinishedSpeech = FinishedSpeech;
  }

  private void FinishedSpeech()
  {
    if (objective.IsActive())
    {
      objective.CompletedObjective();
    }
  }
}