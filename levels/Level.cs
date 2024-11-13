using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.levels;

[JsonObject(MemberSerialization.OptIn)]
public partial class Level : Node
{
  [JsonProperty]
  private Queue<Objective> objectives;
  [JsonProperty]
  private List<Npc> npcs;
  [JsonProperty]
  private Narrator narrator;

  private bool startObjectives;
  //TODO: add entities?

  public Level()
  {
    objectives= new Queue<Objective>();
    npcs = new List<Npc>();
  }

  public void LoadLevel(List<Objective> objectives,List<Npc> npcs,Narrator narrator)
  {
    this.narrator = narrator;
    narrator.NotInteracting = NextObjective;
    objectives.ForEach(objective => this.objectives.Enqueue(objective));
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
      var lines = objectives.Peek().postCompletionFeedback;
      objectives.Dequeue();
      if (lines == null)
      {
        NextObjective();
        return;
      }
      narrator.Narrate(lines);
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