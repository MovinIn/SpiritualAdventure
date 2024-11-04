using Godot;

public class TimedInfoFrame : PlayerInfoFrame
{
    public double delta;
    public double delay;
    public TimedInfoFrame(Vector2 position, Vector2 scale, double delta,double delay=0d) : base(position, scale)
    {
        this.delta=delta;
        this.delay = delay;
    }
}