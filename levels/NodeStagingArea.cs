using Godot;
using System;

public partial class NodeStagingArea : Node
{
  private static NodeStagingArea singleton;
  
  public static void Add(Node n)
  {
    singleton.AddChild(n);
  }

  public static void Remove(Node n)
  {
    singleton.RemoveChild(n);
  }
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    singleton = this;
  }
  
}