using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility.parse;

public static class SpeechParseUtils
{
  private static SpeechLine ParseSpeechLineRecursive(JObject data,JObject extraPointers,DynamicParser parser)
  {
    dynamic dyn = data;
    Identity identity;
    
    if (dyn.speaker != null && dyn.name != null)
    {
      if (!Enum.TryParse((string)dyn.speaker, out Speaker speaker))
      {
        speaker = Speaker.Unknown;
      }
      string name = dyn.name;
      identity = new Identity(speaker,name);
    }
    else
    {
      identity = Identity.Unknown;
    }
    
    var speechLine=new SpeechLine(identity,(string)dyn.content);
    
    if (dyn.nextKey != null)
    {
      speechLine.SetNext(parser.DynamicClone<SpeechLine, JObject>(dyn.nextKey, extraPointers,
        new Func<JObject, SpeechLine>(o => ParseSpeechLineRecursive(o, extraPointers,parser))));
    }

    if (dyn.options != null)
    {
      JArray options = dyn.options;
      Dictionary<string, SpeechLine> optionsDict=new();
      foreach (var token in options)
      {
        JArray option = (JArray)token;
        SpeechLine nextLine=parser.DynamicClone(option[1], extraPointers,
          new Func<JObject, SpeechLine>(o => ParseSpeechLineRecursive(o,extraPointers,parser)));
        optionsDict.Add(option[0].Value<string>(),nextLine);
      }

      speechLine.SetOptions(optionsDict);
    }

    return speechLine;
  }
  
  public static SpeechLine Parse(JObject data,DynamicParser parser)
  {
    return ParseSpeechLineRecursive(data,data,parser);
  }
}