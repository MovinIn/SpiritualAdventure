using Godot;

namespace SpiritualAdventure.entities;

public class Action
{
    public readonly double initialDelay;
    public readonly Vector2 nextPoint;

    public Action(float x,float y,double initialDelay=0d)
    {
        this.initialDelay = initialDelay;
        nextPoint = new Vector2(x,y);
    }
}