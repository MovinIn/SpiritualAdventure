using System;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.objectives;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.utility.parse;

public static class IdentityParseUtils
{
  public static Identity Parse(JObject json)
  {
    if (!Enum.TryParse(json.Value<string>("speaker") ?? Speaker.Archer.ToString(), out Speaker speaker))
    {
      speaker = Speaker.Archer;
    }
    string name = json.OrAsOptional("name", "Narrator");
    return new Identity(speaker, name);
  }  
}