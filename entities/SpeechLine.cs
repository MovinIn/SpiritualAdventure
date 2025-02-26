﻿using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace SpiritualAdventure.entities;

public class SpeechLine:ICloneable<SpeechLine>
{
  public string line { get; private set;}
  
#nullable enable
  
  [JsonProperty("delay", NullValueHandling=NullValueHandling.Ignore)]
  private double? delay;
  
  [JsonProperty("next", NullValueHandling=NullValueHandling.Ignore)]
  public SpeechLine? next;
  
  [JsonProperty("options", NullValueHandling=NullValueHandling.Ignore)]
  public Dictionary<string, SpeechLine>? options;
  
#nullable disable
  
  private static readonly double letterDelay = SpeechDisplay.letterDelay;

  public SpeechLine() { }
  
  public SpeechLine(string line, SpeechLine? next=null)
  {
    Update(line,next);
  }
    
  public SpeechLine(string line, Dictionary<string, SpeechLine>? options)
  {
    Update(line,options);
  }

  [JsonConstructor]
  public SpeechLine(string line, SpeechLine? next, Dictionary<string, SpeechLine>? options)
  {
    Update(line,next,options);
  }

  public void Update(string line,SpeechLine? next=null)
  {
    this.next = next;
    Update(line,(Dictionary<string,SpeechLine>?)null);
  }

  public void Update(string line,Dictionary<string,SpeechLine>? options)
  {
    this.line = line;
    this.options = options;
  }

  public void Update(string line, SpeechLine? next, Dictionary<string, SpeechLine>? options)
  {
    this.next = next;
    Update(line,options);
  }

  public SpeechLine LastLine()
  {
    var l=this;
    while (l.next != null)
    {
      l = l.next;
    }

    return l;
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

  public SpeechLine Clone()
  {
    return new SpeechLine {
      line = line,
      options = options?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
      delay = delay,
      next = next?.Clone()
    };
  }
}