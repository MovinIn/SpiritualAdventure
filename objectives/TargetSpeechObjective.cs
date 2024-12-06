using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.objectives;

public class TargetSpeechObjective:IHasObjective
{
  private readonly string targetLine;
  public Objective objective { get; }

  public bool foundTargetLine;
  private Npc npc;

  public TargetSpeechObjective(Npc npc,Objective objective, string targetLine)
  {
    this.objective = objective;
    this.targetLine = targetLine;
    this.npc = npc;
    foundTargetLine = false;
  }

  public void Start()
  {
    npc.SetInteractHandler(OnInteract);
    npc.NewSpeechLine = NewSpeechLine;
  }
  
  private void OnInteract()
  {
    if (((IHasObjective)this).IsCurrent()&&foundTargetLine)
    {
      objective.CompletedObjective();
    }
    npc.OnInteract();
  }

  private void NewSpeechLine(string line)
  {
    if (((IHasObjective)this).IsCurrent()&&targetLine.Equals(line))
    {
      foundTargetLine = true;
    }
  }
}