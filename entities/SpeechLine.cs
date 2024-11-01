using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritualAdventure.entities;

public class SpeechLine
{
    public string line;
    public double delay;
    public SpeechLine next;
    public Dictionary<string, SpeechLine> options;
    private static readonly double letterDelay = SpeechDisplay.letterDelay;

    public SpeechLine(string line, SpeechLine next) : this(line, new Dictionary<string, SpeechLine>())
    {
        this.next = next;
    }
    
    public SpeechLine(string line, Dictionary<string, SpeechLine> options)
    {
        this.options = options;
        this.line = line;
        delay = line.Length * letterDelay;
    }

    public bool HasNext()
    {
        return next != null;
    }

    public bool Finished()
    {
        return next == null && (options==null||!options.Any());
    }
}