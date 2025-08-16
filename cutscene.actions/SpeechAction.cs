#nullable enable
using SpiritualAdventure.entities;

namespace SpiritualAdventure.cutscene.actions;

public class SpeechAction : DelayedCutsceneAction
{
  private readonly SpeechLine? lines;
  public Narrator narrator { get; }

  public SpeechAction(Narrator narrator, SpeechLine? lines, double initialDelay) : base(initialDelay)
  {
    this.lines = lines;
    this.narrator = narrator;
  }

  protected override void ActAfterDelay()
  {
    narrator.Narrate(lines);
  }
}