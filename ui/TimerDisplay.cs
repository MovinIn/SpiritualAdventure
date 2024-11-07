using Godot;

namespace SpiritualAdventure.ui;

public partial class TimerDisplay : RichTextLabel
{
  private Timer timer;
  private readonly float redThreshold=10f;
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    timer = GetNode<Timer>("Timer");
    timer.Start();
    timer.Paused = true;
    Update(20);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (timer.IsPaused()) return;
    Modulate = new Color(Modulate.R,(float)timer.TimeLeft/redThreshold,
      (float)timer.TimeLeft/redThreshold);
    Text = timer.TimeLeft.ToString("0.00");
  }

  public void Update(float initialTime)
  {
    timer.WaitTime = initialTime;
    timer.Start();
  }

  public void On(bool on)
  {
    timer.Paused = !on;
  }

  public void OnTimerTimeout()
  {
    Text = "0.00";
  }
}