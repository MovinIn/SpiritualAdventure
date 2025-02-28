using SpiritualAdventure.ui;

namespace SpiritualAdventure.objectives;

public struct Identity
{
  public Speaker speaker { get; }
  public string name { get; }

  public Identity(Speaker speaker,string name)
  {
    this.speaker = speaker;
    this.name = name;
  }
}