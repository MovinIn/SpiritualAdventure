using Newtonsoft.Json.Linq;

namespace SpiritualAdventure.utility;

public static class ParseUtils
{
  public static T OrAsOptional<T>(this JObject json, string key,T optional)
  {
    return json.TryGetValue(key, out var tokenValue) ? tokenValue.ToObject<T>() : optional;
  }
}