using Godot;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.entities;

public class MovementAction:Action
{
    public readonly Vector2 nextPoint;

    public MovementAction(float x,float y,double initialDelay=0d) : base(initialDelay)
    {
        nextPoint = new Vector2(x,y);
    }

    public MovementAction(Vector2 nextPoint,double initialDelay=0d): base(initialDelay)
    {
      this.nextPoint = nextPoint;
    }
}