using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class SpeechAction : DelayedCutsceneAction
{
  private SpeechLine lines;
  public Narrator narrator { get; }

  public SpeechAction(Narrator narrator, SpeechLine lines, double initialDelay) : base(initialDelay)
  {
    this.lines = lines;
    this.narrator = narrator;
  }

  protected override void ActAfterDelay()
  {
    narrator.Narrate(lines);
  }
}