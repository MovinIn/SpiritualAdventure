public class PlayerTracker {
    public Replay replay;
    
    public PlayerTracker(){
        reset();
    }

    // we could just reinstantiate the playerTracker instance, so not sure if this is worthit
    public void reset(){
        replay=new Replay();
    }
    public void addFrame(TimedInfoFrame frame){
        replay.add(frame);
    }
}