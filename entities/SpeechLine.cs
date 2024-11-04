using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SpiritualAdventure.entities;

public class SpeechLine
{
  public string line;

  [JsonProperty("delay", NullValueHandling=NullValueHandling.Ignore)]
  private double? delay;

  [JsonProperty("next", NullValueHandling=NullValueHandling.Ignore)]
  public SpeechLine? next;
  
#nullable enable
  [JsonProperty("options", NullValueHandling=NullValueHandling.Ignore)]
  public Dictionary<string, SpeechLine>? options;
    
  private static readonly double letterDelay = SpeechDisplay.letterDelay;

  public SpeechLine(string line, SpeechLine? next=null) : this(line, (Dictionary<string,SpeechLine>?)null)
  {
    this.next = next;
  }
    
  public SpeechLine(string line, Dictionary<string, SpeechLine>? options)
  {
    this.options = options;
    this.line = line;
  }

  [JsonConstructor]
  public SpeechLine(string line, SpeechLine? next, Dictionary<string, SpeechLine>? options)
  {
    this.line = line;
    this.next = next;
    this.options = options;
  }

  public bool HasNext()
  {
    return next != null;
  }

  public bool Finished()
  {
    return next == null && (options == null || !options.Any());
  }

  public void SetDelay(double delay)
  {
    this.delay = delay;
  }
    
  public double GetDelay()
  {
    return delay ?? line.Length*letterDelay;
  }
}