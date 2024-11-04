using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.entities;

public partial class InteractProximityFilter : Node
{
	private static List<Tuple<Interactable,Vector2>> nodes;
	private static Interactable min;
	private static string interactTrigger;
	static InteractProximityFilter()
	{
		nodes = new List<Tuple<Interactable,Vector2>>();
	}

	public static void Add(Interactable interactable, Vector2 position)
	{
		nodes.Add(new Tuple<Interactable,Vector2>(interactable,position));
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
		if (min != null && min.IsInteracting()) return;
		min = Closest(Root.player.Position);
		interactTrigger = min?.GetInteractTrigger();
	}

	public override void _Input(InputEvent @event)
	{
		if (interactTrigger == null) return;
		
		if (@event.IsActionPressed(interactTrigger))
		{
			min.Interact();
		}
	}

	private static Interactable Closest(Vector2 position)
	{
		foreach(Tuple<Interactable,Vector2> i in nodes) {
			i.Item1.HideInteractTrigger();
		}
		
		if (nodes.Count == 0) return null;
		double minDist=nodes[0].Item2.DistanceTo(position);
		Interactable min=nodes[0].Item1;
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
