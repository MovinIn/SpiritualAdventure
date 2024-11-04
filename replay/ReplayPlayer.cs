using System.Collections.Generic;
using Godot;

public class ReplayPlayer {
    private List<TimedInfoFrame> frames;
    private int index;
    private double cumulativeDelta;
    private double cumulativeDelay;
    
    public bool HasNextFrame(){
        return index<frames.Count;
    }

    public PlayerInfoFrame NextFrame(double delta){
        if(!HasNextFrame()){
            return null;
        }
        return Interpolation(delta);
    }

    private PlayerInfoFrame Interpolation(double delta)
    {
        var frame = frames[index];
        
        cumulativeDelay += delta;
        if (cumulativeDelay<frame.delay)
        {
            return new PlayerInfoFrame(frames[index-1].position,frames[index-1].scale);
        }
        
        cumulativeDelta += delta;
        while(cumulativeDelta>frame.delta){
            cumulativeDelta-=frame.delta;
            cumulativeDelay = 0;
            if(++index==frames.Count){
                return frames[^1];
            }
            frame = frames[index];
        }
        
        var previousFrame = frames[index - 1];
        double proportion=cumulativeDelta/frame.delta;
        Vector2 scale=previousFrame.scale;
        Vector2 position=previousFrame.position+
            (float)proportion*(frame.position-previousFrame.position);
        
        return new PlayerInfoFrame(position,scale);
    }

    public void PlayReplay(Replay r){
        index=1;
        frames = r.frames.ConvertAll(frame=>new TimedInfoFrame(frame.position, frame.scale,frame.delta,frame.delay));
    }
}