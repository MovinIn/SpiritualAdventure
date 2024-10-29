using System.Collections.Generic;
using System.Linq;
using Godot;

public class ReplayPlayer {
    private List<TimedInfoFrame> frames;
    private int index;
    private double cumulativeDelta;
    public bool hasNextFrame(){
        return index<frames.Count;
    }

    public PlayerInfoFrame nextFrame(double delta){
        if(!hasNextFrame()){
            return null;
        }
        return interpolation(delta);
    }

    private PlayerInfoFrame interpolation(double delta) {
        while(cumulativeDelta>frames[index].delta){
            cumulativeDelta-=frames[index++].delta;
            if(index==frames.Count){
                return frames[frames.Count-1];
            }
        }
        cumulativeDelta += delta;
        double proportion=cumulativeDelta/frames[index].delta;
        Vector2 scale=frames[index-1].scale;
        Vector2 position=frames[index-1].position+
            (float)proportion*(frames[index].position-frames[index-1].position);
        return new PlayerInfoFrame(position,scale);
    }

    public void playReplay(Replay r){
        index=1;
        frames = r.frames.ConvertAll(frame=>new TimedInfoFrame(frame.position, frame.scale,frame.delta));
    }
}