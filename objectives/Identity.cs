using SpiritualAdventure.ui;

namespace SpiritualAdventure.objectives;

public struct Identity
{
  public Speaker speaker { get; }
  public string name { get; }

  public static readonly Identity Unknown=new (Speaker.Unknown, "Unknown");
  
  public Identity(Speaker speaker,string name)
  {
    this.speaker = speaker;
    this.name = name;
  }
  
  public override string ToString()
  {
      return $"Identity(speaker={speaker}, name={name})";
  }
}