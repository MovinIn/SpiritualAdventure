using System;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.objectives;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility.parse;

public static class IdentityParseUtils
{
  public static Identity Parse(JObject json)
  {
    GD.Print("was here?");
    
    if (!Enum.TryParse(json.Value<string>("speaker") ?? Speaker.Unknown.ToString(), out Speaker speaker))
    {
      speaker = Speaker.Unknown;
    }
    string name = json.OrAsOptional("name", "Unknown");
    return new Identity(speaker, name);
  }  
}