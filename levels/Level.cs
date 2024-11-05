using System.Collections.Generic;
using Godot;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.levels;

public partial class Level : Node
{
  private Queue<Objective> objectives;
  private List<Npc> npcs;

  private bool startObjectives;
  //TODO: add entities?

  public Level()
  {
    objectives= new Queue<Objective>();
    npcs = new List<Npc>();
  }

  public void LoadLevel(Queue<Objective> objectives,List<Npc> npcs)
  {
    this.objectives=objectives;
    this.npcs=npcs;
    foreach (var npc in npcs)
    {
      AddChild(npc);
    }
  }

  public void NextObjective()
  {
    if (objectives.Count == 0) return;
    objectives.Peek().AddChangeHandler(OnObjectiveStatusChanged);
    objectives.Peek().SetAsObjective();
  }

  public void OnObjectiveStatusChanged(Objective.Status status,Objective objective)
  {
    if (status == Objective.Status.Completed)
    {
      GD.Print("Objective Complete!");
      objectives.Dequeue();
      NextObjective();
    }
    else if (status==Objective.Status.Failed)
    {
      GD.Print("Objective Failed!");
      //TODO: show game over splash
    }
    else if (status==Objective.Status.Start)
    {
      GD.Print("Starting New Objective!");
      //TODO: do something
    }
  }
}