using Godot;

public class TimedInfoFrame : PlayerInfoFrame
{
    public double delta;
    public TimedInfoFrame(Vector2 position, Vector2 scale, double delta) : base(position, scale)
    {
        this.delta=delta;
    }
}