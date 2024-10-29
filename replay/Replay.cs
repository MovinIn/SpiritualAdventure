using System.Collections.Generic;

public class Replay {
    public List<TimedInfoFrame> frames;
    public Replay(){
        frames=new List<TimedInfoFrame>();
    }

    public Replay(List<TimedInfoFrame> frames)
    {
        this.frames=frames;
    }
    public Replay(Replay replay) : this(replay.frames) { }

    public void add(TimedInfoFrame frame) {
        frames.Add(frame);
    }
}