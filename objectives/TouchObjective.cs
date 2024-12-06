using System;
using Godot;

namespace SpiritualAdventure.objects;

public partial class TouchObjective : Sprite2D, IHasObjective
{
  private float center = 0.75f;
  private int initialDirection=-1;
  private float amplitude = 0.25f;
  private static readonly float period = 1.5f;
  private double currDelta;
  private const float dissapationPeriod=0.5f;

  private static readonly PackedScene scene;
  
  public Objective objective { get; protected set; }
  
  static TouchObjective()
  {
    scene = ResourceLoader.Load<PackedScene>("res://objectives/objective.tscn");
  }

  private TouchObjective()
  {
    Visible = false;
  }

  public static TouchObjective Instantiate(Objective objective)
  {
    var touchObjective = scene.Instantiate<TouchObjective>();

    touchObjective.objective = objective;
    touchObjective.objective.AddChangeHandler(touchObjective.ObjectiveStatusChangedHandler);
	
    return touchObjective;
  }
  
  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (objective.completed || objective.hardFail)
    {
      Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - (float)(delta / dissapationPeriod));
      if (Modulate.A <= 0)
      {
        QueueFree();
      }
    }

    currDelta += delta;
    var scale=(float)(initialDirection*amplitude*Math.Cos(currDelta*(2*Math.PI)/period) + center);
    if (currDelta >= period)
    {
      currDelta -= period;
    }
    Scale = Vector2.One * scale;
  }

  public virtual void OnBodyEntered(Node2D body)
  {
    if (body is not Player||objective.hardFail||!Visible||!objective.Initiated()) return;
	
    objective.CompletedObjective();
  }

  private void ObjectiveStatusChangedHandler(Objective.Status status,Objective objective)
  {
    if (status == Objective.Status.Initiated && this.objective == objective)
    {
      Visible = true;
    }
  }
}