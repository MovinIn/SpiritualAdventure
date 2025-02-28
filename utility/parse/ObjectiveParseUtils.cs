using System;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.objects;

namespace SpiritualAdventure.utility.parse;

public static class ObjectiveParseUtils
{
  public static Objective Parse(JObject data, DynamicParser parser)
  {
    dynamic dyn = data;
    string description = dyn.description;
    SpeechLine line;
    if (dyn.postCompletionFeedback != null)
    {
      line= dyn.parser.DynamicClone(dyn.postCompletionFeedback, null,
        new Func<JObject, SpeechLine>(o => SpeechParseUtils.Parse(o, parser))); 
    }
    else
    {
      line = null;
    }
    return new Objective(description, line);
  }
}