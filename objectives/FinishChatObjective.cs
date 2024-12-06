using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

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
    if (((IHasObjective)this).IsCurrent())
    {
      objective.CompletedObjective();
    }
  }
}