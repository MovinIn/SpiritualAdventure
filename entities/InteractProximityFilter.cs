using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.levels;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public partial class InteractProximityFilter : Node
{
  private static List<Tuple<TriggerableInteractable,Vector2>> nodes;
  private static TriggerableInteractable min;
  private static string interactTrigger;
  static InteractProximityFilter()
  {
    nodes = new List<Tuple<TriggerableInteractable,Vector2>>();
  }

  public static void Add(TriggerableInteractable interactable, Vector2 position)
  {
    nodes.Add(new Tuple<TriggerableInteractable,Vector2>(interactable,position));
  }

  public static bool Remove(Interactable interactable)
  {
    for (int i=0; i<nodes.Count; i++)
    {
      if (!nodes[i].Item1.Equals(interactable)) continue;
      nodes[i].Item1.HideInteractTrigger();
      nodes.RemoveAt(i);
      return true;
    }
    return false;
  }

  public override void _Process(double delta)
  {
    if (Level.Paused()||Level.isCutscene) return;
    if (min != null && min.IsInteracting()) return;
    min = Closest(Level.player.Position);
    interactTrigger = min?.GetInteractTrigger();
  }

  public static void OnInput(InputEvent @event)
  {
    if (interactTrigger == null||InteractDisplay.IsActive()) return;

    if (@event.IsActionPressed(interactTrigger))
    {
      min.Interact();
    }
  }

  private static TriggerableInteractable Closest(Vector2 position)
  {
    foreach(Tuple<TriggerableInteractable,Vector2> i in nodes) {
      i.Item1.HideInteractTrigger();
    }
		
    if (nodes.Count == 0) return null;
    double minDist=nodes[0].Item2.DistanceTo(position);
    TriggerableInteractable min=nodes[0].Item1;
    for (int i = 1; i < nodes.Count; i++) {
      double dist = nodes[i].Item2.DistanceTo(position);
      if (minDist < dist) {
        min = nodes[i].Item1;
        minDist = dist;
      }
    }
    min.ShowInteractTrigger();
    return min;
  }
}