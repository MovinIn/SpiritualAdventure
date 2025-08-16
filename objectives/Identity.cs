using SpiritualAdventure.ui;

namespace SpiritualAdventure.objectives;

public struct Identity
{
  public Speaker speaker { get; }
  public string name { get; }

  public static readonly Identity Unknown= new Identity(Speaker.Unknown, "Unknown");
  
  public Identity(Speaker speaker,string name)
  {
    this.speaker = speaker;
    this.name = name;
  }
}