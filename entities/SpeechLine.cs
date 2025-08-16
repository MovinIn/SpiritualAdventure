#nullable enable
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.entities;

public class SpeechLine:ICloneable<SpeechLine>
{
  public string line { get; private set;}
  
  private double? delay;
  public SpeechLine? next;
  public Dictionary<string, SpeechLine>? options;
  public Identity identity { get; private set; } 
  
  private static readonly double letterDelay = SpeechDisplay.letterDelay;

  public SpeechLine(Identity identity,string line)
  {
    this.identity = identity;
    this.line = line;
  }
  
  public SpeechLine SetLine(string line)
  {
    this.line = line;
    return this;
  }
  public SpeechLine SetIdentity(Identity identity)
  {
    this.identity = identity;
    return this;
  }
  public SpeechLine SetDelay(double delay)
  {
    this.delay = delay;
    return this;
  }
  public SpeechLine SetNext(SpeechLine next)
  {
    this.next = next;
    return this;
  }
  public SpeechLine SetOptions(Dictionary<string,SpeechLine> options)
  {
    this.options=new Dictionary<string, SpeechLine>(options);
    return this;
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
    
  public double GetDelay()
  {
    return delay ?? line.Length*letterDelay;
  }

  public SpeechLine Clone()
  {
    var clonedLine=new SpeechLine(identity,line);
    
    if (delay != null) clonedLine.SetDelay(delay.Value);
    if (next != null) clonedLine.SetNext(next.Clone());
    if (options != null)
    {
      clonedLine.options= new Dictionary<string, SpeechLine>();
      foreach(var option in options)
      {
        clonedLine.options.Add(option.Key, option.Value.Clone());
      }
    }

    return clonedLine;
  }
}