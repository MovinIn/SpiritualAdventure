#nullable enable
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.entities;

public class SpeechLine:ICloneable<SpeechLine>
{

  public class SpeechLineBuilder
  {
    private string line;
    private double? delay;
    private SpeechLine? next;
    private Dictionary<string, SpeechLine>? options;
    private Identity? identity;

    private SpeechLineBuilder(string line)
    {
      this.line = line;
    }

    public static SpeechLineBuilder Init(string line)
    {
      return new SpeechLineBuilder(line);
    }
    
    public SpeechLineBuilder SetLine(string line)
    {
      this.line = line;
      return this;
    }
    public SpeechLineBuilder SetDelay(double delay)
    {
      this.delay = delay;
      return this;
    }
    public SpeechLineBuilder SetNext(SpeechLine next)
    {
      this.next = next;
      return this;
    }
    public SpeechLineBuilder SetOptions(Dictionary<string,SpeechLine> options)
    {
      this.options=new Dictionary<string, SpeechLine>(options);
      return this;
    }
    public SpeechLineBuilder SetIdentity(Identity identity)
    {
      this.identity = identity;
      return this;
    }

    public SpeechLine Build()
    {
      return new SpeechLine(line,delay,next,options,identity);
    }
  }
  
  
  
  
  public string line { get; private set;}
  
  private double? delay;
  public SpeechLine? next;
  public Dictionary<string, SpeechLine>? options;
  public Identity? identity { get; private set; } 
  
  private static readonly double letterDelay = SpeechDisplay.letterDelay;

  private SpeechLine(string line,double? delay,SpeechLine? next,
    Dictionary<string,SpeechLine>? options,Identity? identity)
  {
    this.line = line;
    this.delay = delay;
    this.next = next;
    this.options = options;
    this.identity = identity;
  }

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